# 🏃‍♂️ Gokstad Friidrettsforening API

Dette prosjektet er en REST API-tjeneste for å håndtere administrasjon av medlemmene i Gokstad Friidrettsforening. API-et er bygget med .NET 9 Controllerbasert API og er også hostet i Azure.

## 🛠 Funksjonalitet
- **Medlems- administrasjon**: Opprett, oppdater, slett og list opp med filter medlemmer.
- **Løps- administrasjon**: Opprett, slett og list opp med filter løp.
- **Påmelding**: Bruker kan melde seg av og på for opprettede løp.
- **Resultat- administrasjon**: Opprett, slett og list opp resultater.

## 🚀 Tekniske spesifikasjoner
- Rammeverk: ASP.NET Core med Entity Framework 8 (da Pomelo ikke er kompatibelt med .Net 9 enda)
- .NET 9 SDK
- MySQL v9.1.0 Database
- Azure for hosting online
- Validering: FluentAPI og DataAnnotations for sikring av data
- Integrasjons- og enhetstester for testing av API
- Logging: Logger til live til konsoll, tekstfiler basert på nivå, og til database.


## 🔑 Autentisering

API-et bruker JSON Web Token (JWT). En token blir generert ved innlogging og inneholder MemberId, Epost, og en unik GUID.

## 🪪 Lisensiering

Dette prosjektet er lisensiert under MIT-lisensen.

## 💬 Logging

Serilog logger til:
- Konsoll
- Tekstfiler som kategoriseres basert på nivå av viktighet. En fil per nivå, per dag.
- Database i tabell 'Logs'.

## 📌 Endepunkter
#### Medlemmer (MembersController)

`GET /api/v1/members` 
- Henter medlemmer med paginering og valgfri søkefunksjonalitet.

`GET /api/v1/members/{id}`
- Returnerer detaljer om et spesifikt medlem, eller 404 Not Found hvis medlemmet ikke finnes.

`POST /api/v1/members/register`
- Tar inn registreringsdata og oppretter et nytt medlem.

`POST /api/v1/members/login`
- Autentiserer medlemmet og returnerer en JWT ved suksess.

`PUT /api/v1/members/update/{id}`
- Oppdaterer informasjon om et medlem.

`DELETE /api/v1/members/delete/{id}`
- Sletter et medlem basert på ID med feilhåndtering for tilgang og manglende data.

### Løp (RacesController)

`GET /api/v1/races`
- Henter løp med paginering og valgfri søkefunksjonalitet.

`POST /api/v1/races/create`
- Oppretter et nytt løp basert på brukerinndata.

`DELETE /api/v1/races/delete`
- Sletter et løp basert på ID. Håndterer feil for tilgang, manglende data, og ukjente unntak.

### Påmelding (RegistrationController)

`POST /api/v1/registration/Register`
- Oppretter en ny påmelding.

`DELETE /api/v1/registration/Unregister`
- Sletter en påmelding.

### Resultater (ResultsController)

`GET /api/v1/results`
- Henter resultater med paginering og valgfri søkefunksjonalitet. LøpsID må velges

`POST /api/v1/results/register`
- Oppretter et nytt resultat basert på brukerinndata.

`DELETE /api/v1/results/delete`
- Sletter et resultat basert på ID. Håndterer feil for tilgang, manglende data, og ukjente unntak.