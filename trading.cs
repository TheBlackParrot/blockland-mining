// You want to trade PLAYER AMOUNT_GIVING ORE_GIVING for AMOUNT_TAKING ORE_TAKING
function serverCmdTrade(%this,%target,%amount_giving,%ore_giving,%amount_taking,%ore_taking) {
	// ALL THE ERROR CHECKS
	if(%target !$= "" && !%amount_giving && %ore_giving $= "" && !%amount_taking && %ore_taking $= "" && %this.trading[being_asked]) {
		switch$(%target) {
			case "yes":
				%this.acceptTrade();
			case "no":
				%this.declineTrade();
		}
		return;
	}

	if(%target $= "" || !%amount_giving || %ore_giving $= "" || !%amount_taking || %ore_taking $= "") {
		messageClient(%this,'',"\c6You're missing some things!");
		messageClient(%this,'',"\c3/trade \c5[target] [amount giving] [ore giving] [amount taking] [ore taking]");
		messageClient(%this,'',"\c7Example: /trade TargetPlayer4839 8 gold 32 copper");
		return;
	}

	if(%amount_giving <= 0 || %amount_taking <= 0) {
		messageClient(%this,'',"\c6You cannot use 0 or negative amounts.");
		%this.play2D(errorSound);
		return;
	}

	if(!isObject(findClientByName(%target))) {
		messageClient(%this,'',"\c6This player doesn't exist!");
		%this.play2D(errorSound);
		return;
	} else {
		%target = findClientByName(%target);
	}
	if(%target == %this) {
		messageClient(%this,'',"\c6You cannot trade with yourself. \c7(Equivalent exchange may be added in the future though!)");
		%this.play2D(errorSound);
		return;
	}

	%ore_giving = strLwr(%ore_giving);
	%ore_taking = strLwr(%ore_taking);
	%ores = "copper coal silver iron gold platinum titanium diamond uranium plutonium solarium aegisalt rubium violium erchius";
	if(stripos(%ores,%ore_giving) == -1) {
		messageClient(%this,'',"\c3" @ %ore_giving SPC "\c6doesn't exist!");
		%this.play2D(errorSound);
		return;
	}
	if(stripos(%ores,%ore_taking) == -1) {
		messageClient(%this,'',"\c3" @ %ore_taking SPC "\c6doesn't exist!");
		%this.play2D(errorSound);
		return;
	}

	if(%this.amount[%ore_giving] < %amount_giving) {
		messageClient(%this,'',"\c6You don't have<color:" @ getOreColor(%ore_giving) @ ">" SPC %amount_giving SPC %ore_giving SPC "\c6to trade!");
		%this.play2D(errorSound);
		return;
	}
	if(%target.amount[%ore_taking] < %amount_taking) {
		messageClient(%this,'',"\c3" @ %target.name SPC "\c6doesn't have<color:" @ getOreColor(%ore_taking) @ ">" SPC %amount_taking SPC %ore_taking SPC "\c6to trade!");
		%this.play2D(errorSound);
		return;
	}

	if(isObject(%this.trading[target])) {
		messageClient(%this,'',"\c6You're already trading with\c3" SPC %this.trading[target].name @ "\c6!");
		%this.play2D(errorSound);
		return;
	}
	if(isObject(%target.trading[target])) {
		messageClient(%this,'',"\c3" @ %target.name SPC "\c6 is already trading with someone!");
		%this.play2D(errorSound);
		return;
	}

	%this.trading[target] = %target;
	%this.trading[ore_giving] = %ore_giving;
	%this.trading[ore_taking] = %ore_taking;
	%this.trading[amount_giving] = %amount_giving;
	%this.trading[amount_taking] = %amount_taking;
	%target.trading[target] = %this;
	%target.trading[ore_giving] = %ore_taking;
	%target.trading[ore_taking] = %ore_giving;
	%target.trading[amount_giving] = %amount_taking;
	%target.trading[amount_taking] = %amount_giving;

	%target.askToTrade(%this);
	messageClient(%this,'',"\c6You asked\c3" SPC %target.name SPC "\c6 to trade... awaiting their response.");
}

function GameConnection::askToTrade(%this,%target) {
	%this.trading[being_asked] = 1;
	messageClient(%this,'',"\c3" @ %this.trading[target].name SPC "\c6would like to trade<color:" @ getOreColor(%this.trading[ore_taking]) @ ">" SPC %this.trading[amount_taking] SPC %this.trading[ore_taking] SPC "\c6for<color:" @ getOreColor(%this.trading[ore_giving]) @ ">" SPC %this.trading[amount_giving] SPC %this.trading[ore_giving]);
	messageClient(%this,'',"\c6Type \c3/trade yes \c6to accept the trade, or \c3/trade no \c6to decline.");

	serverCmdMiningServer_requestGUIVars(%this);
}

function GameConnection::acceptTrade(%this) {
	%target = %this.trading[target];
	%amount_giving = %this.trading[amount_giving];
	%amount_taking = %this.trading[amount_taking];
	%ore_giving = %this.trading[ore_giving];
	%ore_taking = %this.trading[ore_taking];

	if(%this.amount[%ore_giving] < %amount_giving) {
		messageClient(%this,'',"\c6You don't have<color:" @ getOreColor(%ore_giving) @ ">" SPC %amount_giving SPC %ore_giving SPC "\c6to trade!");
		%this.play2D(errorSound);
		%this.declineTrade();
		return;
	}
	if(%target.amount[%ore_taking] < %amount_taking) {
		messageClient(%this,'',"\c3" @ %target.name SPC "\c6doesn't have<color:" @ getOreColor(%ore_taking) @ ">" SPC %amount_taking SPC %ore_taking SPC "\c6to trade!");
		%this.play2D(errorSound);
		%this.declineTrade();
		return;
	}

	%this.amount[%ore_giving] -= %amount_giving;
	%this.amount[%ore_taking] += %amount_taking;

	%target.amount[%ore_giving] += %amount_giving;
	%target.amount[%ore_taking] -= %amount_taking;

	%target.trade[being_asked] = 0;
	%this.trade[being_asked] = 0;
	%this.trading[target] = "";
	%target.trading[target] = "";

	messageClient(%this,'',"\c6You traded\c3" SPC %target.name @ "<color:" @ getOreColor(%ore_giving) @ ">" SPC %amount_giving SPC %ore_giving SPC "\c6for<color:" @ getOreColor(%ore_taking) @ ">" SPC %amount_taking SPC %ore_taking);
	messageClient(%target,'',"\c6You traded\c3" SPC %this.name @ "<color:" @ getOreColor(%ore_taking) @ ">" SPC %amount_taking SPC %ore_taking SPC "\c6for<color:" @ getOreColor(%ore_giving) @ ">" SPC %amount_giving SPC %ore_giving);

	serverCmdMiningServer_requestGUIVars(%this);
	serverCmdMiningServer_requestGUIVars(%target);
}

function GameConnection::declineTrade(%this) {
	%target = %this.trading[%target];

	messageClient(%target,'',"\c3" @ %this.name SPC "\c6has declined your trade offer.");
	%target.play2D(errorSound);

	%this.trading[being_asked] = 0;
	%this.trading[target] = "";
	%target.trading[target] = "";
}