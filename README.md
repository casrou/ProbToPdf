# ProbToPdf
Downloads an exam-friendly, offline, searchable PDF version of the textbook
  > H. Pishro-Nik, "Introduction to probability, statistics, and random processes", available at https://www.probabilitycourse.com, Kappa     Research LLC, 2014. Licensed under [CC BY-NC-ND 3.0](https://creativecommons.org/licenses/by-nc-nd/3.0/deed.en_US)

## Requirements
- `ReLaXed`
```
npm i -g relaxedjs
```
- `pdftk`

Download [PDFtk Server](https://www.pdflabs.com/tools/pdftk-server/)

## How to
The program downloads and generates all the pages as PDF to a folder named `book` on the Desktop.

Then, use `pdftk` to merge the PDFs:

- Open Terminal/Powershell
- Navigate to the `book` folder on the Desktop
- Run
```
pdftk *.pdf cat output output.pdf
```
