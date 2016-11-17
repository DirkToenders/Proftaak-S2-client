# Proftaak-S2-client

##Installatie  
Download ClientLib.dll  
Ga in je c#-programma naar 	Project>Add reference  
Klik op browse en open je gedownloade DLL-bestand.  
Klik op OK.  


##Implementatie  
Client is een static class, je hoeft geen "client = new Client()" te doen.  
de volgende methoden moeten geïmplementeerd worden:  
*Client.Connect(string PCName, string IPAddress) om de verbinding te openen  
*Client.Hit() als iemand jou geraakt heeft  
*Client.Punt() als je iemand geraakt hebt  
de volgende events moeten geïmplementeerd worden:  
*Client.GameStarted  
*Client.GameStopped  
*Client.GamePaused  

[Hoe implementeer ik events?](http://lmgtfy.com/?q=c%23+events)

##Credits  
© Geert Boer™ en Dirk Toenders™