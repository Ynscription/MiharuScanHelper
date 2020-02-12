# Miharu - Scan Helper

A tool to (hopefully) speed up manga translation efforts and ease tentative translation of manga.

## Installing and Running Miharu

### Requirements
* **.Net Framework 4.5** or later must be installed in your system.
* Some features require **Firefox Web Browser** to be installed in your system, but it's not necessary to run the program.
The [binaries](https://github.com/Ynscription/ScanHelper/releases) of Miharu ~~should be~~ are 100% portable and require no installation as long as the requirements are met.

## Getting the Source Code Running

[Tesseract OCR](https://github.com/tesseract-ocr/tesseract#installing-tesseract) must be in your system.

> The default directory checked for Tesseract is `.\Resources\Redist\Tesseract-OCR\`
> 
>If it's not there or in the PATH, Miharu will ask for the Tesseract install directory on the first run.



It is recommended to use Tesseract 4 and substitute the original trained data with these:
 [Japanese](https://github.com/tesseract-ocr/tessdata_best/blob/master/jpn.traineddata) and [Vertical Japanese](https://github.com/tesseract-ocr/tessdata_best/blob/master/jpn_vert.traineddata)
