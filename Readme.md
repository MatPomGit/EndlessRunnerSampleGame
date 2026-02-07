# Endless Runner - Przykładowa Gra

_Aktualna używana wersja Unity: 2019.3_

## Dla Studentów - Pierwsze Kroki

Ten projekt to doskonały punkt wyjścia do nauki tworzenia gier mobilnych w Unity. Jeśli jest to twój pierwszy kontakt z Unity lub tworzeniem gier, nie martw się - wszystko zostało szczegółowo opisane!

### Co to jest "Endless Runner"?

Endless Runner (nieskończony biegacz) to popularny gatunek gier mobilnych, w którym postać automatycznie biegnie do przodu, a gracz kontroluje tylko ruchy w bok, skakanie i zślizgiwanie się. Przykłady takich gier to Temple Run czy Subway Surfers.

## Pobieranie Projektu - Ważne Informacje

To repozytorium używa systemu wersjonowania opartego na tagach. Sprawdź sekcję [Releases](https://github.com/Unity-Technologies/EndlessRunnerSampleGame/releases), aby pobrać kod źródłowy dla konkretnej wersji Unity, lub użyj tagów git do przełączenia się na konkretną wersję (np. `git checkout 18.2`).

### Uwaga dotycząca klonowania

**To repozytorium wykorzystuje Git Large File Support (LFS).**

Git LFS to rozszerzenie do Git, które pozwala przechowywać duże pliki (jak tekstury, modele 3D, dźwięki) w bardziej efektywny sposób. Bez niego nie pobierzesz prawidłowo wszystkich zasobów graficznych i dźwiękowych.

**Aby sklonować to repozytorium poprawnie, musisz najpierw zainstalować git lfs:**

1. Pobierz git lfs stąd: https://git-lfs.github.com/
2. Zainstaluj pobrany program
3. Uruchom w wierszu poleceń: `git lfs install`
4. Teraz możesz sklonować repozytorium: `git clone [adres-repo]`

Twoje klonowanie powinno teraz pobrać pliki LFS prawidłowo. Jeśli używasz graficznego klienta Git, sprawdź jego dokumentację w kwestii obsługi LFS.

## Opis Projektu

Ten projekt to kompletna gra mobilna typu "endless runner" (nieskończony biegacz) stworzona w silniku Unity. 

### Dla Początkujących - Co Znajdziesz w Tym Projekcie?

1. **Kompletny system gry** - cały kod potrzebny do stworzenia działającej gry
2. **System postaci** - różne postacie do odblokowania
3. **System przeszkód** - automatyczne generowanie przeszkód na trasie
4. **UI (interfejs użytkownika)** - menu, sklep, wyniki
5. **System misji** - zadania dla gracza do wykonania
6. **Przedmioty do zbierania** - monety i power-upy
7. **System zapisywania** - zapisywanie postępów gracza

### Dodatkowe Zasoby do Nauki

Możesz znaleźć [projekt na Unity Asset Store](https://assetstore.unity.com/packages/essentials/tutorial-projects/endless-runner-sample-game-87901)
(Uwaga: to starsza wersja nie używająca Lightweight Rendering Pipeline i Addressables, zobacz notatkę na końcu tego pliku)

Plik INSTRUCTION.txt znajduje się w folderze Assets i opisuje najważniejsze elementy projektu - koniecznie go przeczytaj!

Artykuł jest dostępny na [stronie Unity Learn](https://unity3d.com/learn/tutorials/topics/mobile-touch/trash-dash-code-walkthrough), który omawia niektóre części kodu.

Możesz również odwiedzić [wiki projektu](https://github.com/Unity-Technologies/EndlessRunnerSampleGame/wiki), aby uzyskać bardziej szczegółowe informacje o projekcie, jak go zbudować i modyfikować.

## Notatka o tej Wersji z GitHub

Ta wersja zawiera funkcje, których nie ma w wersji opublikowanej w Asset Store:

- **Tutorial** - podstawowy samouczek wyświetlany przy pierwszym uruchomieniu gry (pomaga nowym graczom nauczyć się sterowania)
- **Lightweight Rendering Pipeline** - nowy system renderowania Unity (lżejszy i szybszy dla urządzeń mobilnych)
- **System Addressable** - nowy system zarządzania zasobami, który zastępuje Asset Bundles (pozwala na dynamiczne ładowanie postaci i motywów)

### Co to Znaczy dla Ciebie jako Studenta?

- **Lightweight Rendering Pipeline** - nauczysz się nowoczesnego sposobu renderowania grafiki w Unity
- **Addressables** - poznasz zaawansowany system ładowania zasobów używany w profesjonalnych grach
- **Tutorial System** - zobaczysz jak tworzyć samouczki dla graczy

**Uwaga:** Dokumentacja w wiki jest w trakcie aktualizacji
