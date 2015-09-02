IF EXIST "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" (
	CALL "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" /build Release %~dp0LiveJournalGrabber.sln
) ELSE (
	IF EXIST "C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" (
		CALL "C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" /build Release %~dp0LiveJournalGrabber.sln
	)
)