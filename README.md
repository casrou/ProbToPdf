# ProbToPdf
Downloads an exam-friendly, offline, searchable PDF version of the textbook
  > H. Pishro-Nik, "Introduction to probability, statistics, and random processes", available at https://www.probabilitycourse.com, Kappa     Research LLC, 2014. Licensed under [CC BY-NC-ND 3.0](https://creativecommons.org/licenses/by-nc-nd/3.0/deed.en_US)

## Requirements
- `ReLaXed`
```
npm i -g relaxedjs
```
- `pdfunite`

  Linux: Available

  Windows:
```
  https://blog.alivate.com.au/poppler-windows/
```

## How to
The program downloads and generates all the pages to a folder named `book` on the Desktop. The final pdf is named `output.pdf`.

If you have problems installing `pdfunite`, install [PDFtk Server](https://www.pdflabs.com/tools/pdftk-server/), navigate to the `book`-folder and use 
```
$"pdftk *.pdf cat output output.pdf"
```
