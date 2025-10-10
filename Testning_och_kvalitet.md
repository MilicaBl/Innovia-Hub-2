# Testning och kvalitet
I det här projektet finns flera enhetstester som testar de viktigaste delarna av systemet, framför allt registrering, inloggning och bokning. Därför är det viktigt att ha just de för att känna sig tryggare i att systemet fungerar som det ska. Skulle man bygga vidare på projektet så kan testerna snabbt visa om något har slutat fungera på grund av ändringarna i projektet. 
Testerna är skrivna med xUnit och för AI testet har jag även installerat Moq för att mocka beroenden. 

## Testerna som finns
- Att registrering fungerar när användaren fyller i giltiga uppgifter 
- Att registrering misslyckades när något saknas (tex. Epost)
- Att inloggning fungerar med rätt lösenord
- Att inloggning med fel användaruppgifter misslyckas
- Att bokningen lyckas när användare resurs och datum är giltiga
- Att ingen ska kunna boka något bakåt i tiden 

RecommendationServiceTests är ett test för AI delen där jag kontrollerar att systemet hanterar och tolkar svar från API på rätt sätt. Testet använder Moq för att simulera både databasanrop och AIClient så att kommunikationen med AI tjänsten kan testas utan att faktiskt anropa API. I framtiden kan tetset utökas för att även kontrollera hur systemet reagerar om API anropet misslyckas.

Jag har försökt bygga projektet så att det är enkelt att lägga till nya funktioner. Koden är uppdelad i Controllers, Repositories, och Services. Jag har skapat en AIClient för AI funktioner som kapslar kommunikationen med OpenAIs API och detta gör det enklare att återanvända AI kommunikationen på flera ställen i projektet utan att duplicera kod. 
För säkerheten är det viktigt att inte lägga viktiga nycklar direkt i koden. Därför har jag använt Environment Variables som ligger i .env fil som ignoreras av .gitignore och aldrig skickas till GitHub.
