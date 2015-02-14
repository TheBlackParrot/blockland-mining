function serverCmdUpgrade(%this,%which) {
	%allowed = "power speed";
	%which = strLwr(%which);
	if(stripos(%allowed,%which) != -1) {
		%this.increaseLevel(%which);
	} else {
		messageClient(%this,'',"\c6You can upgrade the following items in your Manipulator:\c3 power, speed");
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