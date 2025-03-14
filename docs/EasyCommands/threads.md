﻿# Threads

Easy Commands has a basic concept of "threads", which allow you to run multiple scripts simultaneously.

Threads also maintain their own separate set of variables, so you can create multiple instances of the same script with different starting parameters and the threads will run independently.

## The Main Thread
EasyCommands has a "Main" thread which is executed every tick until it is complete.  This thread runs the main program and is the starting point for your script.

If your script finishes executing all threads (both queued and async) and you execute it again, the Main thread restarts (unless you supply an argument).

### Queued Threads
EasyCommands keeps a queue of threads to execute, which it executes from first to last (hence, queue).  By default, the Main thread is the first in the queue.  If you queue up more commands to execute using the queue command, those requests will be put on the back of the queue and then executed once the main thread is finished.

This enables you to queue up things for your script to do once it's done with the current task.  This might include building items, creating things from your factory, etc.

Again, only the first queued thread is executed per tick.

By default you are only allowed to have 50 threads queued up at a time.  This limit can be adjusted by editing the EasyCommands script itself using "Edit", but be careful!

Here's a simple example showing Queued Threads in action:

```
:main
queue buildRobot
queue buildRover
queue buildRobot
queue print "Done!"

:buildRobot
set i to 0
until i > 120
  Print "Building a Robot"
  i++

:buildRover
set i to 0
until i > 120
  Print "Building a Rover"
  i++
```

#### Requested Command Threads
When you invoke your EasyCommands script with an argument, either using a button, or "Run with Argument", or cross-grid communication, this creates a new Queued Thread and puts it at the beginning of the Thread Queue.  This effectively pauses the main thread and starts executing the requested command to completion instead.

Note that invoking your EasyCommands script in this way does not terminate the main thread, it just pauses it.  So don't expect the main thread to stop running just because you tried to interrupt it.  If this is the behavior you desire, have your main function sit idle (```wait until false```) or manage your script's "mode" using a global [Variables](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/variables "Variables").

See [Invoking Your Script](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/invoking "Invoking Your Script") for more information on invoking your script.

### Asynchronous Threads

Asynchronous Threads are like queued threads, except that they are each executed once per tick.  

In a given tick, the first queued thread will always execute first, followed by each of the asynchronous threads, in the order that they were created.

By default you are only allowed to have up to 50 asynchronous threads active at any time.  This limit can be adjusted by editing the EasyCommands script itself using "Edit", but be careful!

Here's an example of asynchronous threads in action:

```
:main
async manageInventories
async manageBatteries
async manageAirlocks

:manageAirlocks
Print "Managing Airlocks"
replay

:manageBatteries
Print "Managing Batteries"
replay

:manageInventories
Print "Managing Inventories"
replay
```

## Thread Variables

Each thread maintains its own set of variables, which are only accessible from commands / functions called within that thread.  This allows threads to operate independently and not stomp on each other's variables.  The drawback is that you can't share variables across threads, unless you explicitly make those variables global when setting them.  Global variables are shared across all threads.

When retrieving variable values in a thread, it attempts to look up thread variables before global variables.  So if you name a thread variable the same as a global variable, it will take precedence.

### Passing Thread Variables to Queued/Async Threads

When you create threads using "queued" or "async", the variables in the current thread are copied to the queued/async thread.  This allows you to persist the state of the program to the newly created threads.

Here's a quick example illustrating this.  Note that each thread is printing a different value for i, because when it was created i was a different value.

```
set i to 0
until i > 4
  async printValue
  i++

:printValue
Print "i is: " + i
replay
```

### Requested Command Thread Variables

Since requested commands are given their own thread, they *do not inherit* variables from the main thread.  If you want those commands to have access to variables you will need to make those variables ```global```.

Consider the following script.  It has a variable for "myDoors" since that is used multiple times.  The functions are supposed to be called using a button or similar.

On the surface it seems like it should just work.  However, this script won't work because the requested threads *do not have access* to the variables in the main thread.

In order for the requested commands (via button) to have access to "myDoors", it needs to be ```global```


```
:main
#Bug, needs to be global!
set myDoors to "My Doors"

:openDoors
Print "Opening " + myDoors

:closeDoors
Print "Closing " +  myDoors
```

Fixed:

```
:main
set global myDoors to "My Doors"

:openDoors
Print "Opening " + myDoors

:closeDoors
Print "Closing " +  myDoors
```