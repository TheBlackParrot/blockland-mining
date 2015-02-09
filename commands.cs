function serverCmdUpgrade(%this,%which) {
	%allowed = "power speed";
	%which = strLwr(%which);
	if(stripos(%allowed,%which) != -1) {
		%this.increaseLevel(%which);
	}
}

function serverCmdHelp(%this) {
	%file = new FileObject();
	%file.openForRead($Mining::Root @ "/help.txt");

	while(!%file.isEOF()) {
		messageClient(%this,'',strReplace(%file.readLine(),"%%VERSION",$Mining::Version));
	}

	%file.close();
	%file.delete();
}