# Availability-Calculator

<h2>Summary</h2>

It is often difficult to plans outings with friends. This program helps 
work out when people are available.
<br><br>
Data is loaded in using text files, with the names of the people in the group
as the file names. 

<h2>How to use</h2>
<h3>File Format</h3>
Data should be entered into the file using commas.
> Files should be named after the person they are about.<br>
> e.g. If you are saving John's availability, it should be saved as John.txt


Data should be in the form:
<br>
Start Of Availability Date, Start of Availability Time, End of Availability Date,
End of Availability Time.
<br><br>
If someone is available from 28/12/23 at 8am until 29/12/23 at 8pm they should enter:
<br>28/12/23,08:00,29/12/23,20:00
<br>
>Note: The program is only capable of working with hours, not minutes.

Each new entry of availability should then be placed on a new line.<br><br>
The program will input all possible streaks from the present day forward to the last piece of data available.

<h2>Features</h2>

<h3>Minimum Number of People</h3>
When running the program, it will ask you for the minimum number of people you want.
<br> It will not show a streak where fewer than this number of people are available.

<h3>Minimum Streak</h3>
As you likely do not want a 1 Hour get together, you are able to input the minimum length
of streak that it will show you. Streaks shorter than this will not be shown.
<h3>Active Hours</h3>
This program currently has "Active Hours" built in. This means it will not show a streak
that goes outside these hours, e.g. assuming people are asleep.<br><br>
By default, active hours are 08:00 - 23:00
<h3>Hour by Hour Breakdown</h3>
When a streak has been calculated, it will list every hour of this streak,
and everytime a person starts/stops being available, it will display all the 
names of the people are available from that specific hour.<br><br>
For example if Person1 and Person2 are available from 12:00 - 16:00 on 28/12/23 and Person3
is available from 15:00 - 16:00 it will show:

29/12/2023 09:00 -> 12:00
<ul>
<li>| 12:00 Person1, Person2</li>
<li>| 13:00</li>
<li>| 14:00</li>
<li>| 15:00 Person1, Person2, Person3</li>
<li>| 16:00</li>
</ul>
<h3>Birthday Party</h3>
This only displays streaks where a specific member is available. For example, 
if you are organising a birthday party, you would want the person whose birthday
it is to be there.<br><br>
This feature is only enabled if you specify a minimum number of people smaller than 
the total number of people. You are then able to require as many people to be required
as you like.

<h2>Authors</h2>

This is a project worked on by [william-j-m-m](https://github.com/william-j-m-m) and [FaintLocket424](https://github.com/FaintLocket424)




