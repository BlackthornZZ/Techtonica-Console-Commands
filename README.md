# Techtonica Console Commands
So basically, there are commands now. Enjoy the chaos.

# Commands Reference:

! Open Console: / key !

    give <itemname> <amount>
give (local!) player items. Item name is the display name of the item without spaces.

    setplayerparams <parameter> <value>
set various variables inside the playercontroller. For the parameter choose from choose from: run, walk, fly, jump, scan (speed).

    echo <stringtolog> <logtype>
log a string in the console. For the logtype choose from: info, warning, error, fatal, or message (determines color)

    tp <X> <Y> <Z>
X, Y, and Z coordinates, each can also be replaced with ~ to use current player position (of that component).

    warp <location>
Warp to a specific location, for example "lima", "victor", "freight", "xray", and "waterfall" (waterfall facility)

    unlock <nodename> <bShouldDrawPower>
Unlock a techtree upgrade. Nodename is the name of the techtree node without spaces. For bshoulddrawpower false is recommended. Tip: Use "unlock all false" to quickly unlock everything! (expect A LOT of dialogue spam)

    opensesame
opens whatever door you're currently looking at/near. (in a radius of 8 voxels/powerfloors)

    weightless
toggles weightlessness

    instamole
toggles instamining

    echolocation (DEBUG)
logs your current location in the console. You can use this information to make warps. (Soon to be implemented!)

    cammode <MODE>
changes your camera mode. keep in mind for freecam using noclip is usually more convenient.

    noclip
toggles noclip.

    gamespeed <amount>
Set a multiplier for the game's speed.

    bind <key> <command>{arg1,arg2}
Bind a key to a command. Keys are case sensitive. Use {arg1,arg2} for arguments in the command (no spaces inside of the brackets {}).

    setsize <size> <scaleparams>
Set the player size. Scaleparams for scaling the player parameters such as walk speed.


