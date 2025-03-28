# Rollwagen mit zwei Druckfedern

Done by KPP (pern) in 2025!

For Physics Engines @ ZHAW.

Der Rollwagen hat zwei Druckfedern und st�sst gegen zwei Endpuffer. Je nach bumperMode ist der linke Bumper 
fixiert oder frei, wobei man auch einstellen kann, ob der Bumper reibungsfrei oder mit viskoser Reibung ist.

Die PArameter f�r die Masse, Anfangsgeschwindigkeit, Federl�nge und -konstante sind heikel, verstellt man nur 
einen PAramter zu viel, kracht der Wagen in die Puffer. Bei manchen Einstellungen ist die Integration zu ungenau, 
dann kann man mit den Solver PArametern spielen. Das ist eigentlich ein nettes Addon um im Physikunterricht 
zu zeigen.

Grunds�tzlich stellt man alles im objekt Car ein oder im gleichnamigen Skript. Dort sind auch alle Kr�fte 
definiert, also auch die Kr�fte auf den Bumper. Der Exporter soll helfen die Daten zu exportieren. Diese Klasse 
ist so geschrieben, dass sie m�glichst universell ist, damit man sie auch in anderen Projekten einfach einsetzen 
kann.

To launch the car when recording a video, set recording flag to true in the Car script.

## Mögliche Aufgben
- Eine Zug- und Druckfeder für Einmassenschwinger
- Beide Federn implementieren bumperMode0
    - sind Federn richtig implementiert
    - z,v,a plotten + dl Feder und Federkraft
- bumperMode1
    - elasitscher Stoss, v mit Stossformel vergleichen, m_bumper variieren
    - Impulserhaltung anschauen
    - ev. anstatt der Feder einen "schnellen Stoss" mit den Stossformeln implmentieren (eher nicht OnColission verwenden, sondern eher in FixedUpdate sobald Abstand <1mm)
- bumperMode2
    - teilweise elastischer Stoss
    - Impuls- und Energieerhaltung anschauen, ev. dissipierte Energie im Bumper ebenfalls exportieren


In Teil 3 kommt der inelastische Stoss mit Rotation