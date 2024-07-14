#include <SPI.h>
#include <WiFiS3.h>
#include <LiquidCrystal_I2C.h>
#include <DHT.h>

#include <Arduino.h>
#include "MHZ19.h"
#include <SoftwareSerial.h>

#define RX_PIN 6       // Rx pin which the MHZ19 Tx pin is attached to
#define TX_PIN 7       // Tx pin which the MHZ19 Rx pin is attached to
#define BAUDRATE 9600  // Device to MH-Z19 Serial baudrate (should not be changed)

MHZ19 myMHZ19;                            // Constructor for library
SoftwareSerial mySerial(RX_PIN, TX_PIN);  // (Uno example) create device to MH-Z19 serial
unsigned long getDataTimer = 0;

//  Wi-Fi definitions
char ssid[] = "Galaxy-RC";    //  SSID da rede/
char pass[] = "ttqo3746";     //  password
int status = WL_IDLE_STATUS;  // Wifi radio's status
//  IP da aplicação
char* host = "46.105.31.193";
const int postPorta = 8081;
// Variáveis globais que irão armazenar os valores dos sensores
float h;
float t;
float c;
char* sala = "B09";

const long postDuracao = 10000;  //intervalo entre cada envio para a base de dados
unsigned long ultimoPost = 0;
bool conected = false;
WiFiClient client;
//---------------------
//fim wifi definitions---------------------

// Endereço I2C do LCD e suas dimensões (16 colunas e 2 linhas)
LiquidCrystal_I2C lcd(0x27, 16, 2);  // Altere o endereço I2C conforme o seu display

// Definições para o sensor DHT11
#define DHT11PIN 2     // Pino digital conectado ao DHT11 no Arduino Uno WiFi Rev4
#define DHTTYPE DHT11  // Tipo de sensor DHT

DHT dht(DHT11PIN, DHTTYPE);

/*
 * Método WIFICONNECT para efetuar a ligação à rede
 */
void wificonnect(char ssid[], char pass[]) {
  Serial.begin(9600);  //INICIALIZA A SERIAL
  while (!Serial) {
    ;  // espera pela porta série para conectar.
  }
  delay(500);

  // ver se módulo wifi está presente:
  if (WiFi.status() == WL_NO_MODULE) {
    Serial.println("Communicação com o módulo wifi falhou!");
    while (true)
      ;
  }
  // verificar se o firmware do módulo wireless está atualizado:
  String fv = WiFi.firmwareVersion();
  if (fv < WIFI_FIRMWARE_LATEST_VERSION) {
    Serial.println("Please upgrade the firmware");
  }
  // Tentativa de ligação à Rede Wifi:
  while (status != WL_CONNECTED) {
    Serial.print("Tentativa de ligação à Rede Wifi, SSID: ");
    Serial.println(ssid);
    status = WiFi.begin(ssid, pass);
    // espera 10 segundos para tentar ligar de novo:
    delay(10000);
  }

  Serial.println(WiFi.localIP());
  // definimos a duração do último post e fazemos o post imediatamente
  // desde que o main loop inicía
  ultimoPost = postDuracao;
  Serial.println("Setup completo");
  Serial.println("");
  Serial.println("Está conectado à rede");
}

float GetCO2() {
  // Leitura do co2
  if (millis() - getDataTimer >= 2000) {
    //int CO2;

    /* note: getCO2() default is command "CO2 Unlimited". This returns the correct CO2 reading even 
        if below background CO2 levels or above range (useful to validate sensor). You can use the 
        usual documented command with getCO2(false) */

    c = myMHZ19.getCO2();  // Request CO2 (as ppm)
    return c;
  }
}

void GetReadings() {
    // Leitura da temperatura e Humidade do DHT11
    t = dht.readTemperature();
    h = dht.readHumidity();
    c = GetCO2();

  // Verifica se a leitura foi bem-sucedida
  if (isnan(h) || isnan(t) || isnan(c)) {
    lcd.setCursor(0, 0);
    lcd.print("Ocorreu um problema");
    lcd.setCursor(0, 1);
    lcd.print("no DHT11!");
    delay(2000);
    return;
  }
  // Mostra as leituras no LCD
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Temperatura: ");
  lcd.print(t);
  lcd.print(" C");

  lcd.setCursor(0, 1);
  lcd.print("Humidade: ");
  lcd.print(h);
  lcd.print(" %");

  delay(5000);
  lcd.clear();
  lcd.setCursor(0, 0);

  lcd.print("CO2: ");
  lcd.print(c);
  lcd.print(" ppm");


  // Atraso entre as leituras
  delay(2000);
}

void send_readings() {
  Serial.begin(9600);
  Serial.println("A iniciar o envio de dados");
  Serial.print(" A conectar-se a ");
  Serial.println(host);
  Serial.print(" na porta ");
  Serial.println(postPorta);

  GetReadings();
  // Chamar o URL/End-point as aplicação web
  String url = String("/Home/SaveReadings?") +
  String("temp=") + String(t) +
  String("&humidity=") + String(h) +
  String("&co2=") + String(c) +
  String("&sala=") + String(sala);

  Serial.println(" - A solicitar o URL: ");
  Serial.print("     ");
  Serial.println(url);

  // envia o request/pedido para o servidor
  client.print(String("GET ") + url + " HTTP/1.1\r\n" + "Host: " + host + "\r\n" + "Connection: close\r\n\r\n");
  //delay(500);

  // Lê todas as linhas de resposta que vem do servidor web
  // e escreve no serial monitor
  Serial.println("Resposta do SERVIDOR: ");
  while (client.available()) {
    String line = client.readStringUntil('\r');
    Serial.print(line);
  }
  Serial.println("");
  Serial.println(" Fechar a conexão");
  Serial.println("");

  Serial.println("Envio de Dados - FIM");
  Serial.println("");
}


void setup() {
    //chama a função para conectar ao wifi
  wificonnect(ssid, pass);

  Serial.begin(9600);  // Device to serial monitor feedback

  mySerial.begin(BAUDRATE);  // device to MH-Z19 serial start
  myMHZ19.begin(mySerial);   // *Serial(Stream) refence must be passed to library begin().

  myMHZ19.autoCalibration();  // Turn auto calibration ON (OFF autoCalibration(false))

  // Inicialização do LCD
  lcd.init();       // Inicializa o LCD
  lcd.backlight();  // Liga a luz de fundo do LCD
  lcd.setCursor(0, 0);
  lcd.print("A iniciar...");

  // Inicialização do sensor DHT11
  dht.begin();
  delay(2000);
}

void loop() {

if (client.connect(host, postPorta)) {

    unsigned long diff = millis() - ultimoPost;
    if (diff > postDuracao) {
      send_readings();
      ultimoPost = millis();
    }
  } else {

    Serial.println("Não foi possível conectar ao servidor!");
    delay(1000);
  }
}
