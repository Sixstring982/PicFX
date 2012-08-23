PicFX
=====

C# Picture Glitcher

2012 Sixstring982

1. Purpose
	PicFX is a command based image processing program. Glitched pictures can
	easily be created with careful use of the editing library.
	
2. Use
	First, a picture must be loaded. Simply type:
	
		load [filename]
		
	in order to get your picture in PicFx. Once you have it loaded, you
	can see it by typing:
	
		show
	
	When in the command line, there are many commands which will change
	the look of your picture. They are documented in section 4.
	
3. Tutorial
	Let's make a sweet image. First, run PicFx and start by loading a sample.
	
		load samples/man.jpg
	
	After each command, type
	
		show
		
	to see the image at that point. Now, let's try using some of the tools
	to make it look better. First type:
	
		cthreshold 128 r
	
	This gets rid of all red in the image less than 128. Looks pretty snazzy!
	Now, I feel like taking out all of the green.
	
		dechannel g
		
	Let's compress that into a lower bpp now:
	
		compress 6
		
	And now, let's pixelate what we have.
	
		deres 5
		
	Looking pretty badass! Here's a fun trick that yeilds some cool results:
	
		flip v
		pane 8
		flip v
		pane 8
		
	Already looks a little glitchy if you ask me! Now we should add the green
	layer back to the photo:
	
		rechannel g
		
	And perhaps finish it with a compress and another deres
	
		compress 4
		deres 2
	
	And there we go! You probably have the hang of how the program works
	by now.
	
4. Commands
	I. File Browser
		PicFX has a mediocre built in file browser so that you don't get
		lost looking for your pictures. 
		
		cd [path]
		Change directory to the path listed. May be a little buggy, but
		it seems to work well when I use it.
		
		dir
		See what is inside the current folder. Folders are blue and files
		are yellow.
		
		load [filename]
		Loads an image for editing.
		
		save [filename]
		Saves the edited image at the path specified.
		
		unload
		Unloads the file from PicFx. Useful if you want to open the
		image that you are currently editing elsewhere.
		
		show
		Shows the picture in a new form, allowing you to view your changes.
		
		cam
		Shows the picture in a cam window, with a camMap window nearby.
		When clicked, the camMap window will allow you to browse around
		the loaded image.
		
	II. Control
		PicFx has commands which edit the flow of editing.
		
		for [int loopCount] [command]
		Performs the command loopCount times, then stops.
		
		rscript [filename]
		Runs a script, saved at filename. See section 5 for details.
		
		exit
		quit
		Closes the program. Don't forget to save!
		
	III. Picture Editing
		The fun part!
		
		deres [int scale]
		Turns the picture into one with larger pixels. scale determines
		the size of each pixel.
		
		compress [int scale]
		Compresses the picture into a lower bpp. Each increment in scale
		divides the bpp by two, for instance: a 32bpp picture compressed
		with a scale of 3 goes down to an 8bpp picture.
		
		negate
		Takes all channels and makes each the negative version of itself.
		
		reset
		Resets the picture back to how it was when you loaded it.
		
		m+
		Shuffles the picture forward based on a predetermined path. Can
		be used to encrypt the picture in some way, because m- is the
		inverse function of m+ .
		
		m-
		Shuffles the picture backwards based on the mPath. Since it
		is the inverse of m+, using these in the right way will let
		you encrypt your picture.
		
		m
		Performs m+ and m- in order on your picture. If a picture
		does not load correctly, this may fix it.
		
		crotate
		Rotates each color channel to another channel. I.e., 
		R = B, B = G, G = R
		
		pane [int size]
		Selects strips of size pixels wide, and reverses them. Does this
		for the whole picture.
		
		csmear
		Performs a smattering of calls to the editing library. Makes
		your image look pretty fascinating most of the time
		
		flip [v | h]
		Flips the picture, either horizontally or vertically based
		on its arguments.
		
		dechannel [g | r | b]
		Removes one of the color channels from the image.
		
		rechannel [g | r | b]
		Replaces one of the color channels with the original values from
		when the picture was loaded.
		
		multiply [double value] [r | g | b]
		Multiplies each of the values in the specified channel by the
		value argument.
		
		greychannel [r | g | b]
		Sets each of the other channels equal to the channel specified
		in the argument. Greyscales the picture.
		
		cthreshold [byte threshold] [r | g | b]
		If a pixel's channel value does not meet the threshold, it is
		set to 0.
		
		ckeep [byte threshold] [r | g | b]
		Runs cthreshold on the two unspecified channels.
		
		clow [byte threshold] [r | g | b]
		If a pixel's channel value meets the threshold, it is set to
		0.
		
		Shift
		[rshift | lshift | ushift | dshift] [r | g | b | all] [int distance]
		Shifts the specified channel in the calling direction by
		distance pixels.
		
5. Scripting

	PicFX supports some basic scripting. All you must do is use the rscript
	command in conjunction with the filename of the script that you wish to
	run.
	
	A script consists of a series of any of the commands listed in section
	four. If you wish to see an example, there is a sample script called
	demo.pfs located in the samples folder.
	    
		
Happy glitching!

--Sixstring982
		