# Innovia Hub – Intranät och bokningssystem

Detta repo innehåller projektarbetet för kursuppgiften **Innovia Hub**.

## Om uppgiften

Innovia Hub är ett intranät och bokningssystem för coworkingcentret Innovia Hub. Systemet är byggt för att underlätta vardagen för både medlemmar och administratörer.

Länken till den publicerade appen 

<a href="https://innovia-96376.ondigitalocean.app">https://innovia-96376.ondigitalocean.app/</a>


För användaren
- Medlemmar kan logga in och boka resurser i realtid, som skrivbord, mötesrum, VR-headsets och AI-servrar.
- Systemet visar aktuellt tillgängliga tider och uppdateras automatiskt via SignalR när någon annan gör en bokning – användaren ser direkt om en tid blir upptagen.
- En responsiv och enkel frontend gör att systemet kan användas på dator, surfplatta och mobil.
- En AI baserad rekommendationsfunktion som hjälper användaren att hitta lämpliga resurser och tider baserat på tillgänglighet och tidigare bokningar.

För administratören
- Administratörer har en egen panel där de kan hantera användare, resurser och bokningar.
- De kan aktivera/inaktivera resurser, ta bort bokningar eller uppdatera information om medlemmar.
- All data hanteras via ett API som backend tillhandahåller.

Tekniska funktioner
- Backend är byggt i ASP.NET Core med Entity Framework Core och Identity för autentisering och behörigheter.
- Bokningar och användare lagras i en SQL-databas (MySQL).
- Realtidskommunikation sker med SignalR, vilket gör att alla användare får live-uppdateringar utan att behöva ladda om sidan.
- Frontend är byggd i React (Vite) och kommunicerar med backend via ett REST API och en SignalR-klient.

IoT Sensorer - Behöver intalleras separat 
- En admin har tillgång till sensorerna
- Sensorer får data var 10:e sekund
- Om datan matchar specifika regler visas en alert med informationen
Du kan läsa mer om hur du skapar sensorer här:
```bash
https://github.com/MilicaBl/innovia-iot
```
## Vår Stack

- **Backend:** ASP.NET Core (C#)
- **Testning: xUnit och Moq (för mockning av beroenden)**
- **Frontend:** React (Vite)
- **Databas:** SQL (MySQL via Docker)
- **Realtidskommunikation:** SignalR
- **API:** Mockat sensor-API

---

## Kom igång – Installation (Backend + Frontend)

Krav på verktyg/versioner
- **.NET SDK:** 9.0
- **Node.js:** 18 eller 20 rekommenderas
- **Docker:** krävs för att köra MySQL-containern 

Nedan följer en steg-för-steg guide för att köra projektet lokalt.

### 1. Backend

Öppna en terminal i `Backend/` och kör:
```powershell
cd Backend
dotnet restore
dotnet build
dotnet run
```

Backend startar på `http://localhost:5022` (API-bas: `http://localhost:5022/api`).

För att starta databasen:
```bash
docker-compose up -d

docker run --name innovia-mysql \
  -e MYSQL_ROOT_PASSWORD=yourpassword \
  -e MYSQL_DATABASE=Innoviahub \
  -e MYSQL_PASSWORD=yourpassword 
  -p 3307:3306 \
  -d mysql:8

```

Notera:
- Projektet seedar data och en admin-användare vid första körningen (se `Services/DbSeeder.cs`).
- Standard-admin skapas med: användarnamn `admin`, lösenord `Admin@123`, roll `admin`.
- Du kan inte bli admin när du registrerar dig. För att logga in som admin, använd e-postadressen `admin@example.com` och lösenordordet `Admin@123`
- SignalR hub körs på `/bookingHub` och `/recommendation`.
- Databasanslutning styrs av `ConnectionStrings:DefaultConnection` i `Backend/appsettings.json`.
  - Du kan byta port/användare/lösen här eller via user secrets/ miljövariabler.

### 2. Starta Frontend

Frontend använder Vite och läser API-bas via `VITE_API_URL`.

1. Skapa en .env i `Frontend` med:
```env
VITE_API_URL=http://localhost:5022/api
```

Öppna en ny terminal i `Frontend/` och kör:
```powershell
cd Frontend
npm install
npm run dev
```

Frontend startar på `http://localhost:5173` 

---

## Strukturen
- `Backend/` – ASP.NET Core API, EF Core, Identity, SignalR
- `Frontend/` – React + Vite, React Router, SignalR-klient

## Databasen
- Starta MySQL lokalt och säkerställ att konfigurationen matchar `appsettings.json` (se ovan).
- Databasen och seed-data skapas automatiskt första gången du kör backend.

---

## Felsökning

- CORS-fel mellan frontend och backend:
  - Kontrollera att backend tillåter anrop från `http://localhost:5173`.
  - Säkerställ att `VITE_API_URL` pekar på rätt adress (`http://localhost:5022/api`).

- Databasanslutning misslyckas:
  - Verifiera att MySQL kör på port `3307` eller uppdatera `appsettings.json` till din port.
  - Kontrollera användare/lösenord och att databasen finns/kan skapas.
