# GoFold

## What is it?

GoFold is an extension for SSMS 19.

![test](https://drive.google.com/uc?export=view&id=1h_cvlkZKxp0LSaaZGvsQB9K-R_yxnVRd)

## What does it do?

GoFold makes code folding work properly in SSMS (or at least "properly" according to my preferences).

It will look for batches in a query window and fold (collapse) each batch.

The first (non empty) line in the batch will remain visible. 
The "GO" line will be folded along with the rest of the batch.

Batches with two lines of text or less (not including the "GO" line) will not be folded.


## Installation

Unzip Alllmhuran.GoFold.zip into your SSMS extensions directory. 

Once unzipped you should have an \Extensions\Allmhuran.GoFold subdirectory.

The default directory for SSMS 19 is C:\Program Files (x86)\Microsoft SQL Server Management Studio 19\Common7\IDE\Extensions


## The finer details

Only "GO" is currently recognised as a batch separator. If you change your batch separator in SSMS the folding will no longer work.

The extension does not currently try to avoid finding "GO" lines in comments. If you have a line of text with just "GO" at the start of the line inside a comment, the comment will be folded up to and including the go.

