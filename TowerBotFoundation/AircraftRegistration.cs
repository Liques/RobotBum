﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerBotFoundation
{
    public class AircraftRegistration
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public bool IsValid { get; set; }

        public AircraftRegistration(string registration)
        {
            try
            {
                this.Name = registration;
                this.Country = GetCountryRegistration(registration);
                this.IsValid = !String.IsNullOrEmpty(registration);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Aircraft Registration error");
            }
        }


        private static string GetCountryRegistration(string registration)
        {

            string jsonstring = @"
{
  'YA-': 'Afeganistão',
  'ZA-': 'Albânia',
  'T7-': 'Argélia',
  'C3-': 'Andorra',
  'D2-': 'Angola',
  'VP-A': 'Anguilla',
  'V2-': 'Antígua e Barbuda',
  'VE-': 'Argentina',
  'LQ-': 'Argentina',
  'LV-': 'Argentina',
  'EK-': 'Armenia',
  'P4-': 'Aruba',
  'VH-': 'Austrália',
  'OE-': 'Áustria',
  '4K-': 'Azerbaijão',
  'C6-': 'Bahamas',
  'A9C-': 'Bahrain',
  'S2-': 'Bangladesh',
  '8P-': 'Barbados',
  'EW-': 'Belarus',
  'OO-': 'Bélgica',
  'V3-': 'Belize',
  'TY-': 'Benin',
  'VP-B': 'Bermuda',
  'VQ-B': 'Bermuda',
  'A5-': 'Butão',
  'CP-': 'Bolívia',
  'T9-': 'Bósnia e Herzegovina',
  'E7-': 'Bósnia e Herzegovina',
  'A2-': 'Botswana',
  'PP-': 'Brasil',
  'PR-': 'Brasil',
  'PS-': 'Brasil',
  'PT-': 'Brasil',
  'PU-': 'Brasil',
  'VP-L': 'Ilhas Virgens Britânicas',
  'V8-': 'Brunei',
  'LZ-': 'Bulgária',
  'XT-': 'Burkina Faso',
  '9U-': 'Burundi',
  'XU-': 'Cambodia',
  'TJ-': 'Camarões',
  'C-': 'Canadá',
  'D4-': 'Cabo Verde',
  'VP-C': 'Ilhas Cayman',
  'TL-': 'Africano República Central',
  'TT-': 'Chad',
  'CC-': 'Chile',
  'B-': 'China',
  'B-H': 'Hong Kong',
  'B-K': 'Hong Kong',
  'B-L': 'Hong Kong',
  'B-M': 'Macau, China',
  'HJ-': 'Colômbia',
  'HK-': 'Colômbia',
  'D6-': 'Comores',
  'TN-': 'Congo',
  'E5-': 'Ilhas Cook',
  '9Q-': 'Congo',
  'TI-': 'Costa Rica',
  'TU-': 'Côte d Ivoire',
  '9A-': 'Croácia',
  'CU-': 'Cuba',
  '5B-': 'Chipre, República da',
  'OK-': 'República Checa',
  'OY-': 'Dinamarca',
  'J2-': 'Djibouti',
  'J7-': 'Dominica',
  'HI-': 'República Dominicana',
  '4W-': 'Timor Leste',
  'HC-': 'Equador',
  'SU-': 'Egito',
  'YS-': 'El Salvador',
  '3C-': 'Guiné Equatorial',
  'E3-': 'Eritreia',
  'ES-': 'Estónia',
  'ET-': 'Etiópia',
  'VP-F': 'Ilhas Malvinas',
  'DQ-': 'Ilhas Fiji',
  'OH-': 'Finlândia',
  'F-': 'França',
  'F-OG': 'French West Indies',
  'F-O': 'Guiana Francesa',
  'TR-': 'Gabão',
  'C5-': 'Gâmbia',
  '4L-': 'Georgia',
  'D-': 'Alemanha',
  '9G-': 'Gana',
  'VP-G': 'Gibraltar',
  'SX-': 'Grécia',
  'J3-': 'Granada',
  'TG-': 'Guatemala',
  '2-': 'Guernsey',
  '3X-': 'Guiné',
  'J5-': 'Guiné-Bissau',
  '8R-': 'Guiana',
  'HH-': 'Haiti',
  'HR-': 'Honduras',
  'HA-': 'Hungria',
  'TF-': 'A Islândia',
  'VT-': 'Índia',
  'PK-': 'Indonésia',
  'EP-': 'O Irã',
  'YI-': 'Iraque',
  'EI-': 'Irlanda',
  '4X-': 'Israel',
  'I-': 'Itália',
  '6Y-': 'Jamaica',
  'JA': 'Japão',
  'ZJ-': 'Jersey',
  'JY-': 'Jordan',
  'Z6-': 'Kosovo, República da',
  'UP-': 'Kazakhstan',
  '5Y-': 'Quênia',
  'T3-': 'Kiribati',
  'P-': 'Coréia do Norte',
  'HL': 'Coreia do Sul',
  '9K-': 'Kuwait',
  'EX-': 'Quirguistão',
  'RDPL-': 'Laos',
  'YL-': 'Latvia',
  'OD-': 'Líbano',
  '7P-': 'Lesotho',
  'A8-': 'Liberia',
  '5A-': 'Líbia',
  'HB-': 'Liechtenstein',
  'LY-': 'Lituânia',
  'LXN4-': 'Luxemburgo',
  'Z3-': 'Macedónia, República da',
  '5R-': 'Madagascar',
  '7Q-': 'Malawi',
  '9M-': 'Malásia',
  '8Q-': 'Maldivas',
  'TZ-': 'Mali',
  '9H-': 'Malta',
  'M-': 'Isle of Man [14]',
  'V7-': 'Ilhas Marshall',
  'T5-': 'Mauritânia',
  '3B-': 'Maurício',
  'XA-': 'México',
  'XB-': 'México',
  'XC-': 'México',
  'V6-': 'Micronésia',
  'ER-': 'Moldávia',
  '3A-': 'Monaco',
  'JU-': 'Mongólia',
  '4O-': 'Montenegro',
  'VP-M': 'Montserrat',
  'CN-': 'Marrocos',
  'C9-': 'Moçambique',
  'XY-': 'Myanmar',
  'XZ-': 'Myanmar',
  'V5-': 'Namíbia',
  'C2-': 'Nauru',
  '9N-': 'Nepal',
  'PH-': 'Países Baixos',
  'PJ-': 'Antilhas Holandesas',
  'ZK-': 'Nova Zelândia',
  'YN-': 'Nicarágua',
  '5U-': 'Niger',
  '5N-': 'Nigéria',
  'LN-': 'Noruega',
  'A4O-': 'Oman',
  'AP-': 'Paquistão',
  'AR-': 'Paquistão',
  'SU-Y': 'Palestina',
  'E4-': 'Palestina',
  'HP-': 'Panama',
  'P2-': 'Papua Nova Guiné',
  'ZP-': 'Paraguai',
  'OB-': 'Peru',
  'RP-': 'Filipinas',
  'SP-': 'Polônia',
  'SN-': 'Polônia',
  'CR-': 'Portugal',
  'CS-': 'Portugal',
  'A7-': 'Qatar',
  'F-OD': 'Réunion Island',
  'YR-': 'Roménia',
  'RA-': 'Rússsia',
  'RF-': 'Rússsia',
  '9XR-': 'Ruanda',
  'VQ-H': 'Santa Helena / Ascension',
  'V4-': 'São Cristóvão e Nevis',
  'J6-': 'Santa Lúcia',
  'J8-': 'São Vicente e Granadinas',
  '5W-': 'Samoa',
  'T7-': 'San Marino',
  'S9-': 'São Tomé e Príncipe',
  'HZ-': 'Arábia Saudita',
  '6V-': 'Senegal',
  'YU-': 'Sérvia',
  'S7-': 'Seychelles',
  '9L-': 'Sierra Leone',
  '9V-': 'Singapore',
  'OM-': 'Eslováquia',
  'S5-': 'Slovenia',
  'H4-': 'Ilhas Salomão',
  '6O-': 'Somália',
  'ZS-': 'África do Sul',
  'ZT-': 'África do Sul',
  'EC-': 'Espanha',
  'CE-': 'Espanha',
  '4R-': 'Sri Lanka',
  'ST-': 'Sudão',
  'PZ-': 'Surinam',
  '3D-': 'Suazilândia',
  'SE-': 'Suécia',
  'HB-': 'Suíça',
  'YK-': 'Síria',
  'F-OH': 'Tahiti',
  'EY-': 'Tajiquistão',
  '5H-': 'Tanzânia',
  'HS-': 'Tailândia',
  '5V-': 'Togo',
  'A3-': 'Tonga',
  '9Y-': 'Trinidad e Tobago',
  'TS-': 'Tunísia',
  'TC-': 'Turquia',
  'EZ-': 'Turcomenistão',
  'VQ-T': 'Turks e Caicos',
  'T2-': 'Tuvalu',
  '5X-': 'Uganda',
  'UR-': 'Ucrânia',
  'A6-': 'Emirados Árabes Unidos',
  'G-': 'Reino Unido',
  '4U-': 'Nações Unidas',
  'N': 'EUA',
  'CX-': 'Uruguai',
  'UK-': 'Usbequistão',
  'YJ-': 'Vanuatu',
  'YV-': 'Venezuela',
  'VN-': 'Vietnam',
  '7O-': 'Yemen',
  '9J-': 'Zâmbia',
  'Z-': 'Zimbábue'
}
";

            var listCountires = JsonConvert.DeserializeObject<IDictionary<string, string>>(jsonstring);

            string country = String.Empty;
            var countryReg = listCountires.Keys.Where(s => registration.StartsWith(s)).FirstOrDefault();
            countryReg = (String.IsNullOrEmpty(countryReg)) ? "" : countryReg;

            if (listCountires.ContainsKey(countryReg))
            {
                country = listCountires[countryReg];
            }

            return country;

        }

        public override string ToString()
        {
            return this.Name;
        }

    }

    
}
