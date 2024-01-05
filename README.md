# Design und Architektur
Zu Beginn legte ich eine Basisstruktur für den Server fest, der HTTP-Anfragen verarbeiten kann. Mir war es wichtig, eine klare Trennung der Zuständigkeiten vorzunehmen, daher teilte ich die Logik in verschiedene Schichten auf:

##### Controller: Hier definierte ich Endpunkte, die die HTTP-Anfragen bearbeiten.
##### DAL (Data Access Layer): Diese Schicht enthielt Klassen für den Datenzugriff.
##### Interface: Ich erstellte Schnittstellen, um Abhängigkeiten zwischen Komponenten zu minimieren und Testbarkeit zu verbessern.
##### Models: In diesem Bereich entwickelte ich die Kernentitäten wie Karten und Benutzer.
##### Services: Die Geschäftslogik des Spiels wurde hier implementiert.
## Dependency Injection
Ein zentraler Aspekt meines Designs war die Anwendung von Dependency Injection (DI), um die Kopplung zwischen den Klassen zu verringern und eine höhere Flexibilität zu erreichen. DI ermöglichte es mir, Abhängigkeiten zur Laufzeit bereitzustellen, was die UnitTests vereinfachte und die Wartung des Codes verbesserte.


# Herausforderungen und Lösungen
## Unzureichende Model-Definition
#### Problem:
Anfangs waren meine Definitionen in meinem HttpRequest-Modell unzureichend und ineffizient. Es fehlten wichtige Informationen, was zu Problemen bei der Verarbeitung führte.
#### Lösung:
Ich überarbeitete das HttpRequest-Modell, fügte alle notwendigen Informationen hinzu und verbesserte dadurch die Robustheit und Testbarkeit des Systems erheblich.


## Umstrukturierung der Testfälle
#### Problem: Meine ersten Testfälle waren aufgrund der Umwandlung von Json-Strukturen in Klassen obsolet geworden.
####Lösung: Ich aktualisierte die Testfälle, sodass sie mit den neuen Klassen kompatibel waren.

## Json-zu-String-Konvertierung

#### Problem: Die manuelle Konvertierung von Json zu Strings war fehleranfällig.
#### Lösung: Ich verwendete Json-Serialisierung, um den Prozess zu automatisieren und Fehler zu reduzieren.

## Output bei Battle

#### Problem:
Beim Entwickeln der BattleCards-Funktion in meinem Spiel stieß ich auf ein Problem: Die Funktion gab ursprünglich einen Integer-Wert zurück, um das Ergebnis des Kampfes darzustellen. Dies führte jedoch zu Verwirrungen und Missverständnissen, da die Bedeutung der einzelnen Integer-Werte nicht sofort ersichtlich war. Es war nicht intuitiv, ob eine '0', '1' oder ein anderer Wert für einen Gewinner, Verlierer oder ein Unentschieden stand. Diese Mehrdeutigkeit machte den Code schwer zu verstehen und zu warten, was die Fehleranfälligkeit erhöhte.

#### Lösung:
Um dieses Problem zu lösen und den Code klarer und intuitiver zu gestalten, entschied ich mich für die Einführung eines Enums mit dem Namen Winner. Dieses Enum definiert explizit die möglichen Ausgänge eines Kartenduells: Gewinner, Verlierer und Unentschieden. Anstatt also einen abstrakten Integer-Wert zurückzugeben, gibt die BattleCards-Funktion nun einen dieser spezifischen Enum-Werte zurück. Dies macht den Code selbstbeschreibend und verringert das Risiko von Missverständnissen. Wenn ein Entwickler den Code liest oder erweitert, kann er sofort erkennen, was die Rückgabewerte bedeuten

## Fazit
Die Entwicklung des MTCG-Servers war ein Prozess ständiger Anpassung und Verbesserung. Durch das frühzeitige Erkennen von Fehlern, regelmäßige Refaktorisierungen und die Einführung von Dependency Injection konnte ich eine solide, erweiterbare und wartungsfreundliche Basis für das System schaffen.


# Lessons Learned

## Klare Trennung der Zuständigkeiten:

Durch die Aufteilung der Logik in verschiedene Schichten wie Controller, DAL, Interfaces, Models und Services konnte ich die Klarheit und Wartbarkeit des Codes erheblich verbessern. Diese strukturierte Herangehensweise erleichtert das Verständnis des Systems und unterstützt eine effektive Teamarbeit. In Zukunft würde ich früher damit beginnen können, da ich nun eine gutes Verständis habe was welche Schicht übernehmen sollte. 

## Bessere Planung/Definitionen 
Die anfänglichen Unzulänglichkeiten im HttpRequest-Modell zeigten mir, wie kritisch vollständige und präzise Modelldefinitionen sind. Die Überarbeitung dieses Modells verbesserte nicht nur die Robustheit, sondern auch die Testbarkeit des Systems.


## Flexibilität im Testdesign:

Die Notwendigkeit, Testfälle zu aktualisieren, um sie mit neuen Klassen kompatibel zu machen, betonte die Bedeutung von Flexibilität im Testdesign. Es ist wesentlich, Tests so zu gestalten, dass sie leicht anpassbar sind, um mit der Evolution des Codes Schritt zu halten.

## Verwendung von Enums statt random Werten:

Die Entscheidung, Enums anstelle von abstrakten Integer-Werten zu verwenden, um die Ausgänge von Kämpfen darzustellen, verbesserte die Lesbarkeit und Eindeutigkeit des Codes erheblich.
## Einsatz von Exceptions für Klarheit:

Die Verwendung von Exceptions statt boolscher Rückgabewerte für die Erfolgsmeldung von Operationen trug wesentlich zur Verbesserung der Lesbarkeit und Verständlichkeit des Codes bei. Exceptions bieten einen detaillierteren Kontext über Fehlerzustände.
## Vereinfachung des Testens:

Die Erkenntnis, dass einfaches Testen der Applikation entscheidend ist, z.B. nicht das manuelle Setzen eines Admin-Status. Dies erleichtert nicht nur die Durchführung von Tests, sondern trägt auch zur Konsistenz und Zuverlässigkeit der Testergebnisse bei.

# Unit-Test

Um zu erklären, warum ich diese speziellen Unit Tests gewählt habe und warum der getestete Code kritisch ist, fokussiere ich mich auf die zentralen Aspekte der Funktionen und ihre Bedeutung im Gesamtsystem:
## HttpServiceTest:
Ich habe Tests für den HttpService gewählt, da dieser Service die Grundlage für die Kommunikation zwischen Client und Server bildet. Die korrekte Verarbeitung von HTTP-Anfragen ist entscheidend, um sicherzustellen, dass Benutzeranfragen richtig interpretiert und beantwortet werden. Falsch interpretierte oder verarbeitete Anfragen können zu schwerwiegenden Fehlern im System führen. In diesen Tests habe ich vor allem den Fokus auf das parsen des http-Protokolles gesetzt. 
## CardHelperTest:
Der CardHelperTest ist entscheidend, weil er sicherstellt, dass Karten korrekt verarbeitet und in unterschiedlichen Formaten dargestellt werden können. In einem Spiel, das auf Karten basiert, ist es essentiell, dass die Karteninformationen korrekt und konsistent sind, sowohl in der internen Logik als auch in der Kommunikation mit dem Benutzer.
## TestGameService:
Die Tests für den GameService sind kritisch, da sie die Kernlogik des Spiels überprüfen. Besonders das korrekte Funktionieren der Battle-Logik ist entscheidend für das Spielerlebnis. Fehler hier können das Spiel ungerecht oder unspielbar machen. Diese Tests helfen sicherzustellen, dass die Spielregeln korrekt implementiert sind und das Spiel wie erwartet funktioniert. Außerdem sind dies Fehler die schnell übersehen werden können.
## TestPackageService:
Der PackageService ist ein zentraler Bestandteil des Spiels, der dafür verantwortlich ist, neue Kartenpakete zu erstellen und zu verwalten. Die Tests hier stellen sicher, dass Kartenpakete korrekt zusammengestellt werden und dass die Karten den erwarteten Eigenschaften entsprechen. Desweitern überprüfen sie die korrekte Übersetzung der Abfragen in die jeweiligen Klassen. 
## AuthenticationServiceTest:
Ich habe Tests für den AuthenticationService gewählt, weil dieser Service für die Sicherheit und Zugangskontrolle innerhalb der Anwendung verantwortlich ist. Die Authentifizierung ist ein kritischer Baustein in fast jeder Anwendung, da sie sicherstellt, dass nur berechtigte Benutzer Zugriff auf bestimmte Funktionen und Daten erhalten. Dabei wurden sowohl admin zugriffe als auch User zugriffe getestet. 

# Unique Feature
In meinem Projekt habe ich ein uniques Feature implementiert, dass die ELO-Berechnung in den Kampflogiken des Spiels integriert. Dieses Feature ist besonders interessant, da es eine dynamische Bewertung der Spielerfähigkeiten in Echtzeit ermöglicht, basierend auf den Ergebnissen ihrer Kämpfe im Spiel.

Die Implementierung der ELO-Berechnung erfolgt während der Kämpfe zwischen den Spielern. Jeder Spieler hat eine ELO-Bewertung, die sich ändert, je nachdem, ob er gewinnt, verliert oder unentschieden spielt. Die ELO-Veränderung wird durch eine Formel berechnet, die den Unterschied zwischen den ELOs der beiden Spieler berücksichtigt. Dies stellt sicher, dass ein Spieler, der gegen einen deutlich stärkeren Gegner gewinnt, mehr ELO-Punkte erhält als bei einem Sieg gegen einen schwächeren Gegner. Dabei gibt es insgesamt für die Spieler 20 Elo zu verfügung, wenn nun die Spieler gleich stark sind bekommen/verlieren beide 10 und bekommen bei einem Unentschieden 0. Wenn es nun einen sehr großen Abstand gibt bekommt derjene mit der höheren Elo einen 1 Punkt für einen Gewinn, verliert bei 19 Punkte bei einer Niederlage und verliert 9 Punkte bei einem Unteschieden. Für den anderen Spieler ist es immer genau anderes herum, es muss aber immer zumindest 1 Elo Punkt gewonnen bzw verloren egal wie groß der Abstand.

Besonders innovativ ist, wie diese ELO-Änderungen in den Kampflogs reflektiert werden. Nach jedem Kampf wird im Log vermerkt, wie sich die ELOs der beiden Spieler verändert haben. Zudem fördert sie ein kompetitives Umfeld, in dem die Spieler bestrebt sind, ihre ELO-Bewertung zu verbessern. Insgesamt ist diese ELO-Berechnung und -Integration in die Kampflogs ein Schlüsselmerkmal meines Spiels, das es von anderen abhebt und eine tiefergehende Spielerfahrung bietet.

# Track the time spent with the project

### September
10. September: 2 Stunden

15. September: 1 Stunde
    
26. September: 1 Stunde
    

### Oktober
6. Oktober: 1 Stunde

17. Oktober: 2 Stunden
    
24. Oktober bis 31. Oktober: 3 Stunden täglich (21 Stunden gesamt)

### November
1. November bis 3. November: 3 Stunden täglich (9 Stunden gesamt)
   
14. November: 1 Stunde
    
25. November: 1 Stunde
    
### Dezember
5. Dezember: 1 Stunde

16. Dezember: 2 Stunden
    
27. Dezember: 2 Stunden
    
### Januar
1. Januar bis 9. Januar: jeweils 2 Stunden täglich (18 Stunden gesamt)
### Insgesamt (60 Stunden).


# Github
https://github.com/schamori/MTCG/edit/dev/README.md




