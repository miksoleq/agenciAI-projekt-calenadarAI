# calendarAI – architektura i zasady

## Projekty a aplikacja

W .NET są trzy poziomy:

- **Solucja (`.sln`)** – "teczka" grupująca projekty. Sama nic nie robi.
- **Projekt (`.csproj`)** – jedna jednostka kompilacji. Wychodzi z niego albo **biblioteka** (`.dll`, nie da się jej uruchomić), albo **aplikacja** (exe z `Program.cs`).
- **Aplikacja** – ten jeden projekt, który uruchamiamy. Korzysta z bibliotek przez referencje.

```
CalendarAI.sln
├── src/CalendarAI.Core    (biblioteka) model, stan, zapis do JSON, logika kalendarza
├── src/CalendarAI.Agent   (biblioteka) LLM, narzędzia, system prompt  → zależy od Core
└── src/CalendarAI.Cli     (aplikacja)  REPL, tryb app i tryb LLM      → zależy od Core i Agent
```

Po co dzielić na projekty? Kompilator pilnuje kierunku zależności. `Core` nie wie nic o LLM ani o konsoli, więc logikę kalendarza da się testować i podmieniać niezależnie. Przy okazji wychodzi naturalny podział pracy na 3 osoby.

## Stan w JSON

Jeden plik `calendar.json` przechowuje cały stan aplikacji:

- Na start zawiera obiekt `AppState { Version, Events }`.
- Później dochodzą `Tasks` i `Preferences` bez zmiany sposobu zapisu.
- Stan wczytujemy przy starcie, a zapisujemy po każdej zmianie.
- Pole `Version` przyda się, gdy format pliku się zmieni.

## Zasady

### Minimalna konfiguracja i brak magii frameworków

- Bez `Host.CreateDefaultBuilder` i bez kontenera DI. Wszystkie obiekty składamy ręcznie w `Program.cs` (wzorzec _Composition Root_, czyli "pure DI"). Całe okablowanie aplikacji mieści się na ~20 liniach, które każdy rozumie.
- Konfiguracja to jeden mały `appsettings.json` z sekcją `Llm: { Provider, Model, Endpoint }`. Klucz API bierzemy ze zmiennej środowiskowej `OPENAI_API_KEY`, nie z user-secrets.
- Paczki NuGet tylko do LLM:
  - `Microsoft.Extensions.AI` (wspólny interfejs `IChatClient`)
  - `Microsoft.Extensions.AI.OpenAI` (provider OpenAI)
  - `OllamaSharp` (provider lokalny)
- JSON obsługuje wbudowany `System.Text.Json`.

### Kluczowa reguła: oba tryby to tylko dwa wejścia do tej samej logiki

```
 Tryb app:  "add Trening 2026-11-06 18:00 1h" ─┐
                                               ├─→ ICalendarService ─→ IStateStore ─→ calendar.json
 Tryb LLM:  "dodaj trening jutro o 18" → LLM ──┘
            (tool_call add_event → CalendarTools)
```

- Logika kalendarza jest **wyłącznie** w `CalendarService`.
- Komendy CLI i narzędzia LLM to cienkie adaptery: parsują wejście, wołają serwis i formatują wynik. Nie mają własnej logiki.

### Wzorce projektowe

| Wzorzec                  | Gdzie                                                          | Po co                                                                    |
| ------------------------ | -------------------------------------------------------------- | ------------------------------------------------------------------------ |
| Repository / State Store | `IStateStore` → `JsonStateStore`                               | zapis jest wymienny, później można przejść na SQLite bez ruszania reszty |
| Service Layer            | `ICalendarService` → `CalendarService`                         | jedno źródło logiki dla obu trybów                                       |
| Command                  | `ICommand { Name, Help, Execute(args) }` + `CommandDispatcher` | nowa komenda CLI to nowa klasa, bez edycji `switch`a                     |
| Adapter                  | `CalendarTools`                                                | tłumaczy `ICalendarService` na narzędzia dla LLM                         |
| Factory                  | `ChatClientFactory`                                            | tworzy `IChatClient` dla OpenAI albo Ollamy na podstawie configu         |
| Strategy                 | tryby `AppMode` i `AgentMode` za wspólnym interfejsem          | REPL nie wie, który tryb działa                                          |
| Composition Root         | `Program.cs`                                                   | jedyne miejsce, gdzie powstają konkretne obiekty                         |

Dodatkowo `IClock` (aktualny czas). Agent potrzebuje "dzisiejszej daty" do interpretacji słowa "jutro", a w testach podstawiamy stały czas.

### SOLID w praktyce

- **S:** jedna odpowiedzialność na klasę. Serwis nie zajmuje się konsolą, komenda nie zapisuje pliku.
- **O:** nowa komenda albo nowy provider to nowa klasa, a istniejący kod zostaje bez zmian.
- **L:** `JsonStateStore` i np. `InMemoryStateStore` (do testów) muszą być w pełni wymienne.
- **I:** małe interfejsy (`IStateStore` ma tylko `Load` i `Save`).
- **D:** `Cli` i `Agent` zależą od interfejsów z `Core`, nie od konkretnych implementacji.

### Kontrakt (wspólny dla wszystkich, ustalony na start)

- `ICalendarService`: `AddEvent(title, start, end)` → `Event` oraz `ListEvents(from, to)` → lista `Event`
- `Event`: `Id`, `Title`, `Start`, `End`
- `IStateStore`: `Load()` → `AppState` oraz `Save(AppState)`
- `IClock`: aktualny czas
