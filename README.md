# calendarAI

Inteligentny kalendarz, który nie tylko zapisuje wydarzenia i terminy, ale podpowiada, nad czym i kiedy pracować, żeby zdążyć z zadaniami i zachować czas na odpoczynek. Dla osób z napiętym, zmiennym grafikiem, którym prokrastynacja i natłok obowiązków utrudniają samodzielne planowanie.

📄 Pełny opis idei: [vision.md](docs/vision.md)

## Główne funkcje

- **Plan w skali tygodnia, miesiąca i roku** — uwzględnia wydarzenia, sen, weekendy, święta i rozrywkę.
- **Wydarzenia** — stałe i jednorazowe przedziały zajętości lub dostępności (zajęcia, treningi itp.).
- **Zadania z terminem** — deadline i szacowany czas wykonania; deadline'e są wyraźnie oznaczone.
- **Propozycje bloków pracy** — agent układa bloki przed terminami, biorąc pod uwagę wolne okna, priorytety, kontekst dnia oraz czas na dojazd i przygotowanie.
- **Preferencje użytkownika** — agent zapamiętuje m.in. preferowaną długość bloków skupienia i metodę pracy (np. Pomodoro).
- **Agent wyszukujący wydarzenia w okolicy** — na podstawie zainteresowań i obciążenia grafiku proponuje dodatkowe aktywności (koncert, teatr, kino).
- **Elastyczny plan** — to podpowiedź, nie sztywny nakaz; pomaga podejmować bieżące decyzje bez rezygnowania z odpoczynku.

## Uruchomienie
cd /Users/mwelpa/PW/5SEM/AA/agenciAI-projekt-calenadarAI
dotnet run --project src/CalendarAI.Cli/CalendarAI.Cli.csproj

ollama serve
ollama run qwen2.5

demo:
app> help
app> chat
chat> add a workout tomorrow at 6pm
chat> /back
app> list
app> exit

### Wymagania

- .NET 10 SDK
- Ollama z pobranym modelem, np. `ollama run qwen2.5`
- dla OpenAI: zmienna `OPENAI_API_KEY`

### Konfiguracja

Kluczy **nie commitujemy** do repozytorium. Lokalnie używamy user-secrets:

```bash
cd <ścieżka-do-projektu>   # TODO: uzupełnić
dotnet user-secrets init
dotnet user-secrets set "<NazwaKlucza>" "<wartość>"
```

Pozostałe, niewrażliwe ustawienia znajdują się w `appsettings.json`.

### PCSS

Konfiguracja domyślnie korzysta z modelu `Qwen3.5-397B-A17B` przez API PCSS. Przed uruchomieniem ustaw klucz w terminalu:

```bash
export PCSS_API_KEY='twój_klucz'
```

Klucz pochodzi z portalu PCSS. Nie zapisuj go w repozytorium.

### Budowanie i uruchomienie

## Autorzy

Autorzy: Mikołaj Zawrotniak, Maciej Wełpa, Jędrzej Kwaśny
