@ECHO OFF
CLS

SET found=0
SET ext="pdb"

IF EXIST *.%ext% (           rem Check if there are any in the current folder :)
  DEL *.%ext%
  SET found=1
)
FOR /D /R %%G IN ("*") DO (  rem Iterate through all subfolders
  IF EXIST %%G CD %%G
  IF EXIST *.%ext% (
    DEL *.%ext%
    SET found=1
  )
)

IF %found%==1 (
  ECHO.
  ECHO Deleted all .%ext% files.
  ECHO.
) ELSE (
  ECHO.
  ECHO There were no .%ext% files.
  ECHO Nothing has been deleted.
  ECHO.
)

PAUSE
EXIT