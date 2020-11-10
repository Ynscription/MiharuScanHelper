using Miharu.BackEnd.Data;
using Miharu.BackEnd.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Miharu.BackEnd.Translation.HTTPTranslators
{
	class HTTPJishoTranslator: HTTPTranslator
	{
		private const string _URL = "https://jisho.org/search/";
		private const string _SEARCH_URL = "https://jisho.org";


		public override TranslationType Type {
			get { return TranslationType.Jaded_Network; }
		}

		

		protected override string GetUri(string text)
		{
			return _URL
					+ Uri.EscapeDataString(text);
		}


		public string WebGet(string uri)
		{
			string res = "";
			if (uri == "")
				return res;
						
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
			using (HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
				if (response.StatusCode == HttpStatusCode.OK) {
					using (Stream receiveStream = response.GetResponseStream())
					using (StreamReader readStream = 
							new StreamReader(receiveStream, GetEncoding(response.CharacterSet))) {

						res = readStream.ReadToEnd();

						readStream.Close();
					
					}
				}
			}

			return res;
		}


		private JPDictionaryEntry ProcessMeanings (string res, JPDictionaryEntry jpde) {
			

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.DtdProcessing = DtdProcessing.Parse;


			using (Stream s = new MemoryStream(Encoding.UTF8.GetBytes(res))) {
				using(XmlReader reader = XmlReader.Create(s, settings)) {
					bool exit = false;
					int count = 0;
					bool exact = true;
					while (reader.ReadToFollowing("div") && !exit) {
						string nodeClass = reader.GetAttribute("class");
						if (nodeClass == "exact_block" || nodeClass == "concepts") {
							if (nodeClass == "concepts")
								exact = false;
							do {
								if (!reader.ReadToFollowing("div"))
									return jpde;
								nodeClass = reader.GetAttribute("class");
							} while (nodeClass != "concept_light clearfix");


							do {
								
								bool wordDone = false;

								using (XmlReader meaningReader = reader.ReadSubtree()) {
									JPWord word = new JPWord();
									do {
										if (wordDone = !meaningReader.ReadToFollowing("div"))
											break;
										nodeClass = meaningReader.GetAttribute("class");
									} while (nodeClass != "concept_light clearfix");

									if (wordDone)
										continue;

									meaningReader.ReadToFollowing("span"); //furigana
									nodeClass = meaningReader.GetAttribute("class");
									if (nodeClass == "furigana") {
										meaningReader.ReadToFollowing("span");
										nodeClass = meaningReader.GetAttribute("class");
										if (nodeClass != "text") {
											int index = 0;
											
											do {
												using (XmlReader furiReader = meaningReader.ReadSubtree()) {
													furiReader.Read();
													furiReader.ReadStartElement(); //get inside furigana entry node
													if(furiReader.NodeType == XmlNodeType.Text && furiReader.HasValue)
														word.Furigana.Add(new Tuple<int, string>(index, furiReader.Value));
													index++;
												}
											}while(meaningReader.ReadToNextSibling ("span")); //reach Japanese word entry
										}
									}

									
									while (nodeClass != "text") {
										if (wordDone = !meaningReader.ReadToFollowing("span"))
											break;
										nodeClass = meaningReader.GetAttribute("class");
									}
									if (wordDone)
										continue;
									
									do {
										if (wordDone = !meaningReader.Read())
											break;
										nodeClass = meaningReader.GetAttribute("class");
										if (nodeClass != "concept_light-status" && (meaningReader.NodeType == XmlNodeType.Text || (meaningReader.NodeType == XmlNodeType.Element && meaningReader.Name == "span"))) { //check if we are done with text
											if (meaningReader.NodeType == XmlNodeType.Text && meaningReader.HasValue)
												word.Word += meaningReader.Value.Trim();
											else if (meaningReader.Name == "span") {
												meaningReader.Read();
												if (meaningReader.NodeType == XmlNodeType.Text && meaningReader.HasValue)
													word.Word += meaningReader.Value.Trim();
												else if (meaningReader.NodeType == XmlNodeType.Whitespace)
													word.Word += " ";
											}
										}
									}while(nodeClass != "concept_light-status" && meaningReader.Name != "div");

									if (wordDone)
										continue;
									
									do {
										if (wordDone = !meaningReader.ReadToFollowing("div"))
											break;
										nodeClass = meaningReader.GetAttribute("class");
									} while (nodeClass != "meanings-wrapper");

									
									if (wordDone)
										continue;

									string meanings = meaningReader.ReadInnerXml();
									
									
									if (exact)
										jpde.ExactMeanings.Add(new Tuple<JPWord, string>(word, meanings));
									else
										jpde.Concepts.Add(new Tuple<JPWord, string>(word, meanings));
									exit = ++count > 9;

								}
							
									
								
							}while (reader.ReadToNextSibling("div") && !exit);

						}

					}
				}

			}

			return jpde;
		}


		private JPDictionaryEntry ProcessForm (string res, JPDictionaryEntry form) {
			string rootRef = res.Substring(res.IndexOf("<a href=\"") + "<a href=\"".Length);

			int refEndIndex = rootRef.IndexOf("\">");

			string root = rootRef.Substring(refEndIndex + "\">".Length);
			root = root.Substring(0, root.IndexOf("</a>"));

			rootRef = rootRef.Substring(0, refEndIndex);

			


			if (res.Contains("<li>")) {
				form.Forms = new List<string>();
				while (res.Contains("<li>")) {
					res = res.Substring(res.IndexOf("<li>") + "<li>".Length);
					form.Forms.Add(res.Substring(0,res.IndexOf("</li>")).Trim());
				}
			}



			form.FormGuess = ProcessFormOrWord(WebGet(_SEARCH_URL + rootRef));
			form.FormGuess.Word.Word = root;

			return form;
		}


		private JPDictionaryEntry ProcessFormOrWord (string res) {
			JPDictionaryEntry result = new JPDictionaryEntry();

			string form = "";
			if (res.Contains("<div class=\"fact grammar-breakdown\">")) {
				form = res.Substring(res.IndexOf("<div class=\"fact grammar-breakdown\">"));
				form = form.Substring(0, form.IndexOf("</div>") + "</div>".Length);
				result = ProcessForm(form, result);
				
			}
			
			if (res.Contains("<div id=\"primary\" class=\"large-8 columns\">")) {
				res = res.Substring(res.IndexOf("<div id=\"primary\" class=\"large-8 columns\">"));
				res = res.Substring(0, res.IndexOf("<div id=\"secondary\" class=\"large-4 columns search-secondary_column\">"));

				result = ProcessMeanings(res, result);
			}
			


			return result;
		}


		private List<JPDictionaryEntry> ProcessSentence (string res) {
			List<JPDictionaryEntry> sentence = new List<JPDictionaryEntry>();

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.DtdProcessing = DtdProcessing.Parse;


			using (Stream s = new MemoryStream(Encoding.UTF8.GetBytes(res))) {
				using(XmlReader reader = XmlReader.Create(s, settings)) {
					while (reader.ReadToFollowing("li")) {
						if ((reader.GetAttribute("class") ?? "") != "clearfix japanese_word")
							continue;

						using (XmlReader subReader = reader.ReadSubtree()) {
							subReader.ReadToDescendant("span");
							

							JPWord currentWord = new JPWord();

							string nodeClass = subReader.GetAttribute("class") ?? "";
								
							while(nodeClass != "japanese_word__furigana_wrapper") {
								subReader.ReadToNextSibling("span");
								nodeClass = subReader.GetAttribute("class") ?? "";
							};


							
							
							
							/*subReader.ReadToFollowing("span");
							nodeClass = subReader.GetAttribute("class") ?? "";
							if (nodeClass != "japanese_word__text_wrapper") {
								int index = 0;
								do {
									using (XmlReader furiReader = subReader.ReadSubtree()) {
										furiReader.Read();
										if ((furiReader.GetAttribute("class") ?? "") == "japanese_word__furigana") {
											
											furiReader.ReadStartElement();
											if(furiReader.NodeType == XmlNodeType.Text && furiReader.HasValue) {
												currentWord.Furigana.Add(new Tuple<int, string>(index, furiReader.Value.Trim()));
											}
										}
										index++;
									}
								}while(subReader.ReadToNextSibling("span"));
							}*/
							
							while(nodeClass != "japanese_word__text_wrapper") {
								subReader.ReadToFollowing("span");
								nodeClass = subReader.GetAttribute("class") ?? "";
							};
								
								
							subReader.ReadStartElement();
							if(subReader.NodeType == XmlNodeType.Text && subReader.HasValue) {
								string word = subReader.Value;
								int length = JPUTF8CharCounter.StringLength(word);
								currentWord.Word = word;

								sentence.Add(new JPDictionaryEntry(currentWord));
							}
							else {
								do {
									if (subReader.Name == "a") {
										currentWord.Word = subReader.GetAttribute("data-word");
										string word = WebGet(_SEARCH_URL + subReader.GetAttribute("href"));
										JPDictionaryEntry tmp = ProcessFormOrWord(word);
										tmp.Word = currentWord;
									
										sentence.Add(tmp);
										break;
									}
									else if (subReader.Name == "span" && subReader.GetAttribute("class") == "japanese_word__text_with_furigana") {
										subReader.ReadStartElement();
										if(subReader.NodeType == XmlNodeType.Text && subReader.HasValue) {
											string word = subReader.Value;
											int length = JPUTF8CharCounter.StringLength(word);
											currentWord.Word = word;

											sentence.Add(new JPDictionaryEntry(currentWord));
											
										}
										break;
									}
									subReader.Read();
								} while(subReader.NodeType != XmlNodeType.None);
							}

						}
					}
				}
			}

			return sentence;
		}

		protected override string ProcessResponse(string res)
		{
			string result = "";

			if (res.Contains("<section id=\"zen_bar\" class=\"japanese_gothic\" lang=\"ja\">")) {
				res = res.Substring(res.IndexOf("<section id=\"zen_bar\" class=\"japanese_gothic\" lang=\"ja\">"));
				res = res.Substring(0, res.IndexOf("</section>") + "</section>".Length);

				result = JsonConvert.SerializeObject(ProcessSentence(res), Newtonsoft.Json.Formatting.Indented);
			}
			else {
				List<JPDictionaryEntry> r = new List<JPDictionaryEntry>();
				r.Add(ProcessFormOrWord(res));
				result = JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.Indented);
			}
			

			return result;
		}

		protected override Encoding GetEncoding(string received)
		{
			return Encoding.UTF8;
		}
	}
}
