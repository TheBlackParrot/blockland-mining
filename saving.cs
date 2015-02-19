$Mining::SaveDir = "config/server/Mining/saves";

function GameConnection::loadMiningGame(%this) {
	%file = new FileObject();

	%file.openForRead($Mining::SaveDir @ "/" @ sha1(%this.bl_id));
	while(!%file.isEOF()) {
		%line = %file.readLine();
		%type = getField(%line,0);
		%arg1 = getField(%line,1);
		%arg2 = getField(%line,2);
		switch$(%type) {
			case "ore":
				%this.amount[%arg1] = %arg2;
			case "level":
				%this.level[%arg1] = %arg2;
			case "general":
				eval("%this." @ %arg1 @ " = " @ %arg2 @ ";");
			case "date":
				%date = %arg1;
		}
	}

	%file.close();
	%file.delete();

	messageClient(%this,'',"\c6Your save from\c3" SPC %date SPC "\c6has been loaded successfully.");
}

function GameConnection::saveMiningGame(%this) {
	%ores = "copper coal silver iron gold platinum titanium diamond uranium plutonium solarium aegisalt rubium violium erchius";
	
	%file = new FileObject();
	%file.openForWrite($Mining::SaveDir @ "/" @ sha1(%this.bl_id));

	for(%i=0;%i<getWordCount(%ores);%i++) {
		%ore = getWord(%ores,%i);
		%file.writeLine("ore" TAB %ore TAB (%this.amount[%ore] || 0));
	}

	%file.writeLine("level" TAB "power" TAB %this.level[power]);
	%file.writeLine("level" TAB "speed" TAB %this.level[speed]);
	%file.writeLine("level" TAB "range" TAB %this.level[range]);

	%file.writeLine("general" TAB "points" TAB %this.points);

	%file.writeLine("date" TAB getDateTime());

	%file.close();
	%file.delete();
}

package MiningSavingPackage {
	function GameConnection::autoAdminCheck(%this) {
		if(isFile($Mining::SaveDir @ "/" @ sha1(%this.bl_id))) {
			%this.loadMiningGame();
			// checks like this will be taken out later on in the game
			if(!%this.level[range]) {
				%this.level[range] = 1;
			}
		} else {
			%this.level[power] = 1;
			%this.level[speed] = 1;
			%this.level[range] = 1;
			%this.points = 0;
		}
		%this.setCosts();

		return parent::autoAdminCheck(%this);
	}
};
activatePackage(MiningSavingPackage);