@echo off
IF .%1 == . (
 echo "Input argument missing: sourceFile"
 GOTO :eof
)
set sourceFile=%~f1

IF .%2 == . (
 echo "Input argument missing: outputDirectory"
 GOTO :eof
)
set outputDirectory=%~f2

java -Xmx2000m -jar splitter\splitter.jar %sourceFile% --max-nodes=160000 --output-dir=%outputDirectory% --mapid=00000001

:EOF