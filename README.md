# Adherent Sample Oven

This project is software to enable data collection using a USB-DIO96 from Measurment Computing (www.mccdaq.com)

The master branch contains the software to use the device to monitor an oven with up to 30 samples.  It uses a thermocouple to monitor temperature in the oven and records the elapsed time and the final temperature that each sample has been triggered at.

The branch sheer-testing-variant contains a variation of the code that does not worry about temperature.  It does have some additional functionality added:

  - Each sample can be given a sample name.  This name will be recorded in the log file.
  - Each sample can be started and stopped independently.
  - Addition setting was added to determine if the default (non-tripped) state is an open or closed circuit.
 
As a side-effect of this new functionality if you need to change any settings this must be done before any stations are started and then the aplication should be stopped and started again.  
