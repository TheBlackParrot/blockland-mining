function serverCmdUpgrade(%this,%which) {
	%allowed = "power speed";
	if(stripos(%allowed,strLwr(%which)) != -1) {
		%this.increaseLevel(%which);
	}
}