function serverCmdMiningServer_receiveHandshake(%this) {
	if(%this.hasGUI) {
		return;
	}
	echo("SERVER: RECEIVED HANDSHAKE");
	%this.hasGUI = 1;
}

function GameConnection::sendGUIVar(%this,%type,%arg1,%arg2) {
	if(%type $= "") {
		return -1;
	}
	if(!%this.hasGUI) {
		%this.updateBottomPrint();
		return -1;
	}
	commandToClient(%this,'MiningClient_receiveVar',%type,%arg1,%arg2);
}

function serverCmdMiningServer_requestGUIVars(%this) {
	if(!%this.hasGUI) {
		%this.updateBottomPrint();
		return -1;
	}
	for(%i=0;%i<OreList.getCount();%i++) {
		%ore = strLwr(OreList.getObject(%i).type);
		%this.sendGUIVar("ore",%ore,%this.amount[%ore]);
	}
	%this.sendGUIVar("position",%this.player.getPosition());
	%this.sendGUIVar("health",%this.player.health,%this.maxHealth);
	%this.sendGUIVar("score",%this.points);
}

package MiningGUIPackage{
	function GameConnection::autoAdminCheck(%this) {
		echo("SERVER: SENT HANDSHAKE");
		commandToClient(%this,'MiningClient_receiveHandshake');
		return parent::autoAdminCheck(%this);
	}
};
activatePackage(MiningGUIPackage);