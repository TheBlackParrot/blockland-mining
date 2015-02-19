function serverCmdUpgrade(%this,%which) {
	%allowed = "power speed range";
	%which = strLwr(%which);
	if(stripos(%allowed,%which) != -1) {
		%this.increaseLevel(%which);
	} else {
		messageClient(%this,'',"\c6You can upgrade the following items in your Manipulator:\c3" SPC %allowed);
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

function serverCmdInfo(%this,%target) {
	%target = findClientByName(%target);
	if(!isObject(%target)) {
		messageClient(%this,'',"\c6This player doesn't exist!");
		return;
	}

	%ores = "copper coal silver iron gold platinum titanium diamond uranium plutonium solarium aegisalt rubium violium erchius";
	for(%i=0;%i<getWordCount(%ores);%i++) {
		%ore = getWord(%ores,%i);
		if(%target.amount[%ore]) {
			%amount_str = %amount_str @ "<color:" @ getOreColor(%ore) @ ">" @ strUpr(getSubStr(%ore,0,1)) @ "\c6" @ %target.amount[%ore] @ " ";
		}
	}

	messageClient(%this,'',"\c4==--" SPC %target.name SPC "--==");
	for(%i=0;%i<getWordCount(%amount_str);%i+=6) {
		messageClient(%this,'',getWords(%amount_str,%i,%i+6));
	}
	if(isObject(%target.player)) {
		messageClient(%this,'',"<color:ffaaaa>HP\c6:" SPC %target.player.health @ "/" @ %target.maxHealth);
		%pos = %this.player.getPosition();
		%x = mFloor(getWord(%pos,0));
		%y = mFloor(getWord(%pos,1));
		%z = mFloor(getWord(%pos,2));
		messageClient(%this,'',"<color:44ccff>Position\c6:" SPC %x @ "x" SPC %y @ "y" SPC %z @ "z");
	}
	messageClient(%this,'',"<color:22ffaa>Power Level\c6:" SPC %target.level[power]);
	messageClient(%this,'',"<color:22ffaa>Speed Level\c6:" SPC %target.level[speed]);
	messageClient(%this,'',"<color:22ffaa>Range Level\c6:" SPC %target.level[range]);
	messageClient(%this,'',"<color:aaff22>Player Level\c6:" SPC %target.level);
	messageClient(%this,'',"<color:aaff22>EXP Points\c6:" SPC %target.exp SPC "/" SPC getLevelCost(%target.level));
	messageClient(%this,'',"<color:ccaaff>Score\c6:" SPC %target.points);
}

function serverCmdBlocks(%this,%which) {
	%allowed = "biome ore liquid";
	if(%which $= "" || stripos(%allowed,%which) == -1) {
		messageClient(%this,'',"\c6You can list the following types of blocks:\c3" SPC %allowed);
		return;
	}

	%which = strLwr(%which);
	if(stripos(%allowed,%which) != -1) {
		%list = %which @ "List";
		for(%i=0;%i<%list.getCount();%i++) {
			%row = %list.getObject(%i);
			messageClient(%this,'',"\c6" @ %i+1 @ ". <color:" @ RGBToHex(getColorIDTable(%row.color)) @ ">" @ %row.type);
		}
	}
}