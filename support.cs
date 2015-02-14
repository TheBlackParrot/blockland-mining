function RGBToHex(%rgb) {
	%rgb = getWords(%rgb,0,2);
	for(%i=0;%i<getWordCount(%rgb);%i++) {
		%dec = mFloor(getWord(%rgb,%i)*255);
		%str = "0123456789ABCDEF";
		%hex = "";

		while(%dec != 0) {
			%hexn = %dec % 16;
			%dec = mFloor(%dec / 16);
			%hex = getSubStr(%str,%hexn,1) @ %hex;    
		}

		if(strLen(%hex) == 1)
			%hex = "0" @ %hex;
		if(!strLen(%hex))
			%hex = "00";

		%hexstr = %hexstr @ %hex;
	}

	if(%hexstr $= "") {
		%hexstr = "FF00FF";
	}
	return %hexstr;
}

function getOreColor(%ore) {
	for(%i=0;%i<OreList.getCount();%i++) {
		%row = OreList.getObject(%i);
		if(strLwr(%row.type) $= %ore) {
			break;
		}
	}
	return RGBToHex(getColorIDTable(%row.color));
}

// keeping in case of things
function Player::getDirectionFacing(%this) {
	%va = getWord(%this.getForwardVector(),0);
	%vb = getWord(%this.getForwardVector(),1);

	if(mAbs(%va) > mAbs(%vb)) {
		if(%va > 0) {
			return 0; // north
		} else {
			return 2; // south
		}
	} else {
		if(%vb > 0) {
			return 1; // east
		} else {
			return 3; // west
		}
	}

	return -1;
}

function Player::getDirectionUpDown(%this) {
	%ud = getWord(%this.getEyeVector(),2);
	if(%ud >= 0) {
		return 0; // up
	} else {
		return 1; // down
	}
}