# Miharu - Scan Helper

A tool to (hopefully) speed up manga translation efforts and ease tentative translation of manga.
</br>
</br>
## Installing and Running Miharu

### Requirements
* **.Net Framework 4.5** or later must be installed in your system.
* Some features require **Firefox Web Browser** to be installed in your system, but it's not necessary to run the program.
The [binaries](https://github.com/Ynscription/ScanHelper/releases) of Miharu ~~should be~~ are 100% portable and require no installation as long as the requirements are met.

</br>  

## Getting the Source Code Running

[Tesseract OCR](https://github.com/tesseract-ocr/tesseract#installing-tesseract) must be in your system.  
> The default directory checked for Tesseract is `.\Resources\Redist\Tesseract-OCR\`  
>If it's not there or in the PATH, Miharu will ask for the Tesseract install directory on the first run.

It is recommended to use Tesseract 4 and substitute the original trained data with these:
 [Japanese](https://github.com/tesseract-ocr/tessdata_best/blob/master/jpn.traineddata) and [Vertical Japanese](https://github.com/tesseract-ocr/tessdata_best/blob/master/jpn_vert.traineddata)  
</br>  
**Optionally** [Gecko Driver](https://github.com/mozilla/geckodriver/releases/tag/v0.28.0) is required for the web crawling features.  
> geckodriver.exe must be located in the directory `.\Resources\Redist\GeckoDriver\`
 
 </br>
 
## Licenses
 
**Miharu Scan Helper** is licensed under the [MIT License](https://github.com/Ynscription/MiharuScanHelper/blob/master/LICENSE)

Miharu Scan Helper uses the following software as libraries
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) under this [license](https://github.com/Ynscription/MiharuScanHelper/blob/master/Miharu%20Scan%20Helper/Resources/Licenses/Newtonsoft.Json%20LICENSE.md)
* [Ookii.Dialogs](http://www.ookii.org/software/dialogs/) under this [license](https://github.com/Ynscription/MiharuScanHelper/blob/master/Miharu%20Scan%20Helper/Resources/Licenses/Ookii.Dialogs%20%20LICENSE)
* [WpfAnimatedGif](https://github.com/XamlAnimatedGif/WpfAnimatedGif) under this [license](https://github.com/Ynscription/MiharuScanHelper/blob/master/Miharu%20Scan%20Helper/Resources/Licenses/WpfAnimatedGif%20LICENSE)
* [Selenium](https://www.selenium.dev/) under this [license](https://github.com/Ynscription/MiharuScanHelper/blob/master/Miharu%20Scan%20Helper/Resources/Licenses/Selenium%20LICENSE)
* [MahApps.Metro](https://mahapps.com/) under this [license](https://github.com/Ynscription/MiharuScanHelper/blob/master/Miharu%20Scan%20Helper/Resources/Licenses/MahApps.Metro%20LICENSE)
* [MahApps.Metro.IconPacks](https://github.com/MahApps/MahApps.Metro.IconPacks) under this [license](https://github.com/Ynscription/MiharuScanHelper/blob/master/Miharu%20Scan%20Helper/Resources/Licenses/MahApps.Metro.IconPacks%20LICENSE)

The following software may be distributed and used externally by Miharu Scan Helper
* [TesseractOCR](https://github.com/tesseract-ocr/tesseract) under this [license](https://github.com/Ynscription/MiharuScanHelper/blob/master/Miharu%20Scan%20Helper/Resources/Licenses/Tesseract%20OCR%20LICENSE)
* [Gecko Driver](https://github.com/mozilla/geckodriver) under this [license](https://github.com/Ynscription/MiharuScanHelper/blob/master/Miharu%20Scan%20Helper/Resources/Licenses/GeckoDriver%20LICENSE)

Miharu Scan Helper uses the [KozakuraDB](https://github.com/Ynscription/KozakuraDB) under this [license](https://github.com/Ynscription/MiharuScanHelper/blob/master/Miharu%20Scan%20Helper/Resources/Licenses/Kozakura%20LICENSE)
   This  file uses material from the [radkfile](http://nihongo.monash.edu//kradinf.html) and [kanjidic2](http://www.edrdg.org/wiki/index.php/KANJIDIC_Project) dictionary files.
   These files are property of the [Electronic Dictionary Research and Development Group](http://www.edrdg.org/) and are used in accordance with the licence provisions of the Electronic Dictionaries Research Group. see http://www.edrdg.org/edrdg/licence.html

### Translation Sources
Jisho https://jisho.org/  
Yandex.Translate https://translate.yandex.com/  
Google Translate https://translate.google.com/  
Bing Translator https://www.bing.com/translator/  
DeepL Translator https://www.deepl.com/translator  
The Jaded Network http://thejadednetwork.com/sfx  
