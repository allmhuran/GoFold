# GoFold

## What is it?

GoFold is an extension for SSMS 19 and SSMS 20.

![test](https://drive.google.com/uc?export=view&id=1h_cvlkZKxp0LSaaZGvsQB9K-R_yxnVRd)

## What does it do?

GoFold makes code folding work properly* in SSMS (*according to my preferences).

It will look for batches in a query window and fold (collapse) each batch.

The first (non empty) line in the batch will remain visible. 
The "GO" line will be folded along with the rest of the batch.

Batches with only one or two lines of text (not including the "GO" line) will not be folded.


## Installation

Unzip Alllmhuran.GoFold.zip into your SSMS extensions directory. 

Once unzipped you should have an \Extensions\Allmhuran.GoFold subdirectory.

The default directory for SSMS 19 is C:\Program Files (x86)\Microsoft SQL Server Management Studio 19\Common7\IDE\Extensions

The default directory for SSMS 20 is C:\Program Files (x86)\Microsoft SQL Server Management Studio 20\Common7\IDE\Extensions


## The finer details

Only "GO" is currently recognised as a batch separator. If you change your batch separator in SSMS the folding will no longer work.

The extension does not currently try to avoid finding "GO" lines in comments. If you have a line of text with just "GO" at the start of the line inside a comment, the comment will be folded up to and including the go.

## Why does this exist?

SSMS is apparently meant to be able to fold code "automatically", but it has never really worked for me.

In addition, even when it does work the way it is supposed to, it collapses using begin/end blocks. But that's not typically very useful in T-SQL. Not all objects use (or even can use) begin/end blocks.

To me it seems much more natural to fold at batch terminators, especially since I typically work in a deployment script containing multiple create or alter object statements.

It is possible to manually hide code in SSMS by selecting a block of text and using edit > outlining > hide selection, (which is exactly the command that this addin will invoke) but this won't persist across file close/open, making it laborious.

