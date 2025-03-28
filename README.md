# Rollwagen mit zwei Druckfedern

Done by KPP (pern) in 2025!

For Physics Engines @ ZHAW.

Der Rollwagen hat zwei Druckfedern und stösst gegen zwei Endpuffer. Je nach bumperMode ist der linke Bumper 
fixiert oder frei, wobei man auch einstellen kann, ob der Bumper reibungsfrei oder mit viskoser Reibung ist.

Die PArameter für die Masse, Anfangsgeschwindigkeit, Federlänge und -konstante sind heikel, verstellt man nur 
einen PAramter zu viel, kracht der Wagen in die Puffer. Bei manchen Einstellungen ist die Integration zu ungenau, 
dann kann man mit den Solver PArametern spielen. Das ist eigentlich ein nettes Addon um im Physikunterricht 
zu zeigen.

Grundsätzlich stellt man alles im objekt Car ein oder im gleichnamigen Skript. Dort sind auch alle Kräfte 
definiert, also auch die Kräfte auf den Bumper. Der Exporter soll helfen die Daten zu exportieren. Diese Klasse 
ist so geschrieben, dass sie möglichst universell ist, damit man sie auch in anderen Projekten einfach einsetzen 
kann.

To launch the car when recording a video, set recording flag to true in the Car script.
