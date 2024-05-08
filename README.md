# TrueFalseExceptionChecker
Vishnu-Demo-Checker, wechselt konfigurierbar zwischen verschiedenen Rückgabewerten (true, false, null, Exception).
Wird in diversen Demo- und Testjobs genutzt.
Siehe z.B. ...\VishnuHome\Documentation\DemoJobs\Simple\CheckTreeEvents.

## Einsatzbereich

  - Dieses Repository gehört, wie auch alle anderen unter **InPlug** liegenden Projekte, zum
   Repository **Vishnu** (https://github.com/VishnuHome/Vishnu) und ist auch nur für den Einsatz mit Vishnu konzipiert.

## Voraussetzungen

  - Läuft auf Systemen ab Windows 10.
  - Entwicklung und Umwandlung mit Visual Studio 2022 Version 17.8 oder höher.
  - .Net Runtime ab 8.0.2.

## Schnellstart

Die einzelnen Module (Projekte, Repositories) unterhalb von **InPlug** sind von Vishnu und teilweise auch voneinander abhängig,
weshalb folgende Vorgehensweise für die erste Einrichtung empfohlen wird:
  - ### Installation:
	* Ein lokales Basisverzeichnis für alle weiteren WorkFrame-, Vishnu- und sonstigen Hilfs-Verzeichnisse anlegen, zum Beispiel c:\Users\<user>\Documents\MyVishnu.
	* [init.zip](https://github.com/VishnuHome/Setup/raw/master/Vishnu.bin/init.zip) herunterladen und in das Basisverzeichnis entpacken.

	Es entsteht dann folgende Struktur:
	
	![Verzeichnisse nach Installation](./struct.png?raw=true "Verzeichnisstruktur")

## Quellcode und Entwicklung analog zum Repository [Vishnu](https://github.com/VishnuHome/Vishnu)

Für detailliertere Ausführungen sehe bitte dort nach.

## Kurzhinweise

1. Forken des Repositories **TrueFalseExceptionChecker** über den Button Fork
<br/>(Repository https://github.com/InPlug/TrueFalseExceptionChecker)

2. Clonen des geforkten Repositories **TrueFalseExceptionChecker** in das existierende Unterverzeichnis
	.../MyVishnu/**InPlug**
	
