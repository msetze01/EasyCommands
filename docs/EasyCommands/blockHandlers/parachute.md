﻿# Parachute Block Handler
Description
Reference: [Parachutes](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/beacon "Parachutes")

* Block Type Keywords: ```chute, parachute```
* Block Type Group Keywords: ```chutes, parachutes```

Default Primitive Properties:
* Numeric - Height

Default Directional Properties
* Up - Ratio

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Parachute"
set "My Parachute" to enabled
turn on "My Parachute"

#Disable Block
disable "My Parachute"
set "My Parachute" to disabled
turn off "My Parachute"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Parachute"
power on "My Parachute"

#Turn off
turn off "My Parachute"
power off "My Parachute"
```

## "Open" Property
* Primitive Type: Bool
* Keywords: ```open, opened```
* Inverse Keywords: ```close, closed, shut```

Gets/Sets whether the parachute is open (deployed).

```
if "My Parachute" is open
  Print "Safe Landing"

open "My Parachute"
Print "Just kidding!"
close "My Parachute"
```

## "Trigger" Property
* Primitive Type: Bool
* Keywords: ```trigger, triggered, deploy, deployed```

Same as Open.  Gets/Sets whether the parachute is open (deployed).

```
if "My Parachute" is deployed
  Print "Parachute Deployed!"

deploy "My Parachute"
```

## "Auto" Property
* Primitive Type: Bool
* Keywords: ```auto```

Get/Sets whether the parachute will deploye once it reaches it's Auto Deployment Height.

```
if "My Parachutes" are on auto
  Print `Don't worry, I'm sure they will deploy, right?`

set "My Parachutes" to auto
```

## "Limit" Property
* Primitive Type: Numeric
* Keywords: ```limit, limits, distance, distances, range, ranges```

Gets/Sets the distance from the ground at which to auto-deploy.  Note that this property does not directly enable auto-deployment (use Auto property for that).

```
Print "Deployment Height: " + "My Parachute" distance

#500 meters oughta be enough, right?
set "My Parachute" distance to 500
```

## "Height" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```height, heights, level, levels```

Returns the parachute's current height above the ground, in meters.  If not in a gravity well, will return -1.

```
Print "Parachute height: " + "My Parachute" height
```

## "Ratio" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```ratio, ratios, percentage, percentages, percent, percents```

Returns a value between 0 - 1 representing how far open the parachute is (0 = fully closed, 1 = fully open).

```
Print "Open Ratio: " + "My Parachute" ratio
```

## "Velocity" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```velocity, velocities, speed, speeds, rate, rates```

Returns the speed at which the parachute is travelling, in m/s.  Useful if you're approaching the ground in a hurry and want to set the auto deployment height appropriately.

```
Print "Descent Speed: " + "My Parachute" speed
```

## "Gravity" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```gravity, gravities, strength, strengths, force, forces```

Returns the current gravity strength, in m/s.  Useful for calculating when you need to open your parachute based on your heigh, velocity, and force of gravity.

```
Print "Gravity Strength: " + "My Parachute" gravity
```