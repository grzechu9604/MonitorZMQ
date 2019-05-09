Aby włączyć program z przykładową konfiguracją należy przekopiować pliki z ~\Monitor\Configuration\ExampleConfigFiles do folderu z plikiem binarnym oraz włączyć 5 instancji programu, a dla każdej z nich wprowadzić inną liczbę od 1 do 5 z klawiatury.

Przykładowy program pokazuje wykorzystanie rozproszonego monitora w problemie czytelników (3) i pisarzy (@).

Wzajemne wykluczanie procesów zostało zealizowane przy użyciu zegraów Lamporta.

Komunikacja między procesami odbywa się przez przesyłanie komunikatów binarnych przy użyciu biblioteki ZeroMQ.

Zaimplementowane zostały operacje P(Acquire) i V(Release) dla monitorów oraz Wait, Signal i SignalAll dla zmiennych warunkowych.

Przetwarzanie rozpoczyna się od centralnego przydziału numerów identyfikacyjnych procesów.

Operacja monitor wykonując operację Acquire wysyła do wszystkich informację, że chce wejść do sekcji krytycznej.
Czeka na zgody od wszystkich procesów, a jeżeli któryś z procesów odmówi procesowi, to zostaje on zawieszony do czasu uzyskania sygnału, że proces, który niepozwala wejść do sekcji krytycznej, sam zwolnił sekcję krytyczną. Następnie ponawia żądanie. Jeżeli uzyska zgody od wszystkich, to wchodzi do sekcji krytycznej.

Proces otrzymawszy prośbę o pozwolenie wejścia do sekcji krytycznej (klasa MessageHandler) pozwala na wejście innemu procesowi do sekcji krytycznej tylko gdy:
a) sam nie ubiega się o dostęp do danej sekcji krytycznej
b) sam już wcześniej pozwolił procesowi pytającemu na dostęp do sekcji krytycznej (na żądanie o tym samym czasie zegara Lamporta)
c) ubiega się o dostęp do sekcji krytycznej, ale żądanie zostało wysłane z mniejszą wartością zegara Lamporta lub czasy są równe, ale identyfikator procesu pytającego jest mniejszy niż procesu, który otrzymał żądanie.

Proces wykonując operację Release wysyła wszystkim procesom wartości zmiennych warunkowych zwalnianego monitora oraz informację, że nie jest już w sekcji krytycznej.

Proces wykonując operację Wait wysyła informację do wszystkich procesów, aby został zakolejkowany w liście procesów oczekujących na wskazanej zmiennej warunkowej. Następnie wykonuje operację Release i przechodzi w stan uśpienia, aż otrzyma wiadomość budzącą. Po obudzeniu wykonuje operację Acquire, aby znów mieć możliwość wejścia do sekcji krytycznej. Nie ma gwarancji, że obudzony proces pierwszy wejdzie do sekcji krytycznej po otrzymaniu wiadomości budzącej.

Proces wykonując operację Signal i SignalAll wysyła specjalny rodzaj wiadomości. Wysyłając wiadomość Signal, proces wskazuje, który z procesów chce wybudzić (jest to proces pierwszy na liście procesów oczekujących na danej zmiennej warunkowej).