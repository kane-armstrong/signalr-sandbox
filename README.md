# signal-r-scale-out

Quick sandbox for experimenting with scaling out SignalR

## Setup

Local services:

* Redis

Design:

* One Web API instance with a service bus topic subscription and a SignalR hub, configured using dockercompose to run
in two instances
* Two console apps that connect to the hub - OneFish talks to one instance and joins the onefish group, TwoFish talks to
the other instance and joins the twofish group
* The idea is to observe the onefish hub try to notify the twofish client while its connected to the other instance, 
or vice versa, yet have the message still make it to the client anyway thanks to the backplane

Going to see what this looks like for the different backplane options, i.e. code + infrastructure configuration 
requirements (if we're using Azure Service Bus as a backplane, what permissions do we need, etc.) 
