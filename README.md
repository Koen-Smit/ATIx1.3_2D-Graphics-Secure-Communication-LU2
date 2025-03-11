# Avans HBO Jaar 1, Periode 3: ATIx ICT-B1.3 2D-Graphics and Secure-Communication LU2(2024-25)
- (30-01-2025 / 12-03-2025)
- C#, Azure, Unity, Postman, MSTests

## Project Overview
Dit is het derde project van mijn Informatica-opleiding. Het project bestaat uit een individueel en een groepsdeel. Dit is het individuele deel (LU2).

De opdracht: een eigen API ontwikkelen waarmee we data kunnen ophalen, bewerken en opslaan. Vervolgens wordt de API in de cloud geplaatst en gebruikt in een Unity front-end.
- **Unity gedeelte(frontend)**, folder: `LU2-Unity`
- **API gedeelte(backend)**, folder: `LU2-WebApi`
- **MSTests(testen)**, folder: `LU2-WebApi.Tests`

Elke van deze onderdelen krijgt een aparte sectie waarin ik het verder behandel in deze README.

Preview:
![Demo](Assets/Gif/delete-world.gif)

- [Opdracht periode](Assets/Opdrachtbeschrijving.pdf)
- [Start ERD](Assets/Erd_db.PNG)

## Beveiliging
Om extra aandacht te geven aan een van de grotere onderwerpen van deze periode, namelijk beveiliging, heb ik naast verschillende aanvullende veiligheidsmaatregelen (zoals HTTPS, rate-limiting en andere technieken) ook online tests uitgevoerd om te verifiëren hoe goed mijn API-verbinding beveiligd is. Zie deze hieronder de scores van deze test:

![Img](Assets/SSL-report.PNG)
![Img](Assets/security-header-check.PNG)

Natuurlijk waren de resultaten niet meteen perfect, maar ik heb mijn applicatie zodanig aangepast dat deze nu wel aan de vereiste beveiligingsnormen van deze testen voldoet!

---

## Applicatiestructuur/Functionaliteit/Demo

### Frontend(Unity):
Er zijn 4 verschillende "Scenes" in Unity waar je toegang toe hebt:
- Login/Register scherm
- Wereld selector scherm
- Wereld scherm (de editor)
- Share selector (overzicht van met jou gedeelde werelden)

**1. Login/Register scherm**
- Bij alle schermen waar je niet ingelogd bent, word je automatisch teruggestuurd naar het login scherm.
- Hier kun je inloggen of registreren. De naam moet uniek zijn en het wachtwoord moet voldoen aan bepaalde eisen.
- Nadat je succesvol bent ingelogd, wordt het Wereld selector scherm ingeladen. Ook heb je via een instellingen knop de keuze voor een logout.

![Demo](Assets/Gif/Login-Register.gif)

**2. Wereld selector scherm**
- Na inloggen kun je kiezen uit al bestaande werelden of nieuwe werelden aanmaken.(Maximaal 5 werelden kunnen tegelijkertijd bestaan per user.)
- Werelden worden gepresenteerd als planeten met de wereld namen. Als je minder dan 5 werelden hebt, zullen de overgebleven plaatsen gelabeld worden als "create world".
- Wanneer je op "create world" klikt, kun je een naam kiezen en een nieuwe wereld aanmaken.

**3. Wereld scherm (de editor)**
- Als je op een planeet klikt, word je doorgestuurd naar de editor, waar je de zojuist geklikte wereld kunt bewerken.

In de editor heb je verschillende opties:
- Verwijder de wereld inclusief objecten.
- Deel de wereld met anderen.
- Plaats of bewerk objecten in de wereld. **(Wijzigingen worden pas doorgevoerd wanneer je op de "Opslaan" knop drukt. Anders gaan de wijzigingen verloren.)**

Objecten plaatsen en bewerken:
- Klik op een object in de menubalk om het toe te voegen aan de wereld.
- Sleep het object naar de gewenste locatie.
- Gebruik `+` en `-` om de grootte van het object aan te passen.
- Gebruik `R` om het object te roteren.
- Wanneer je het object loslaat, wordt het geplaatst in de wereld.
- `Dubbelklikken` op een object verwijdert het **(let op: pas bij opslaan wordt het object echt verwijderd).**

![Demo](Assets/Gif/Create-edit-save-world.gif)
![Demo](Assets/Gif/Delete-Object.gif)
![Demo](Assets/Gif/delete-world.gif)

**4. Share selector (overzicht van gedeelde werelden)**
- Als je een wereld hebt gedeeld, is deze zichtbaar voor anderen in de Share selector.
- Via de Wereld selector kun je naar dit overzicht navigeren.
- In dit overzicht zie je alle werelden die met jou gedeeld zijn.
- Als je op een gedeelde wereld klikt, laad je deze in de editor, maar met 
- Je kunt de gedeelde wereld alleen bekijken, niet bewerken. (De editor opties zijn uitgeschakeld voor gedeelde werelden.)

![Demo](Assets/Gif/Share-world.gif)

### Backend(C# API):
Lokaal gebruik ik SwaggerUI om de functionaliteiten van mijn API helder en overzichtelijk weer te geven:

![Img](Assets/swagger.PNG)

Hierboven staan alle API-calls. Alles werkt op basis van de User-ID, wat betekent dat je geen omgevingen van andere gebruikers kunt ophalen, tenzij deze expliciet met jou zijn gedeeld. Zelfs dan kun je ze alleen bekijken, niet bewerken.

**Voorbeeld:** Het ophalen van scenes gebeurt op basis van de User-ID. Een gebruiker kan alleen zijn eigen (maximaal 5) werelden ophalen. Is de gebruiker niet ingelogd? dan krijg je een melding dat je geen rechten hebt.

- **Code voor de api staat in de folder: `/LU2-WebApi`**

### Tests(MSTests & Postman):


