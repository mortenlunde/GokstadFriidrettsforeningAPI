-- Generere eksempeldata for tabellen Members
INSERT INTO Members (FirstName, LastName, Email, Gender, Address_Street, Address_PostalCode, Address_City, DateOfBirth, Created, Updated, HashedPassword)
VALUES
    ('Ola', 'Nordmann', 'ola.nordmann@example.com', 'M', 'Karl Johans gate 1', 0154, 'Oslo', '1985-04-12', NOW(), NOW(), '$2b$12$abcdefghijABCDEFGHIJ12345678'),
    ('Kari', 'Nordmann', 'kari.nordmann@example.com', 'F', 'Dronningens gate 10', 7011, 'Trondheim', '1990-11-23', NOW(), NOW(), '$2b$12$12345abcdefghABCDEabcDEFGHIJ67890'),
    ('Per', 'Hansen', 'per.hansen@example.com', 'M', 'Bergen gate 5', 5003, 'Bergen', '1978-03-30', NOW(), NOW(), '$2b$12$mnopqrstMNOPQRSTUVWX1234567890'),
    ('Anne', 'Larsen', 'anne.larsen@example.com', 'F', 'Nygata 7', 4013, 'Stavanger', '1988-06-15', NOW(), NOW(), '$2b$12$yZ12345opqrstuvwxyzABCDEFGHIJKLM'),
    ('Jon', 'Olsen', 'jon.olsen@example.com', 'M', 'Strandveien 2', 3040, 'Drammen', '1995-09-12', NOW(), NOW(), '$2b$12$abcdefghijABCDEFGHIJ12345678'),
    ('Eva', 'Johansen', 'eva.johansen@example.com', 'F', 'Markveien 25', 0554, 'Oslo', '1982-12-01', NOW(), NOW(), '$2b$12$abcdefghijABCDEFGHIJ12345678'),
    ('Lars', 'Nilsen', 'lars.nilsen@example.com', 'M', 'Storgata 30', 8006, 'Bodø', '1992-07-20', NOW(), NOW(), '$2b$12$abcdefghijABCDEFGHIJ12345678'),
    ('Siri', 'Pettersen', 'siri.pettersen@example.com', 'F', 'Hovedveien 12', 1440, 'Drøbak', '1986-05-05', NOW(), NOW(), '$2b$12$abcdefghijABCDEFGHIJ12345678'),
    ('Tom', 'Eriksen', 'tom.eriksen@example.com', 'M', 'Fjordgata 8', 9008, 'Tromsø', '1993-08-18', NOW(), NOW(), '$2b$12$abcdefghijABCDEFGHIJ12345678'),
    ('Maria', 'Berg', 'maria.berg@example.com', 'F', 'Havnegata 14', 4836, 'Arendal', '1987-10-09', NOW(), NOW(), '$2b$12$abcdefghijABCDEFGHIJ12345678');

-- Generere eksempeldata for tabellen Races
INSERT INTO Races (RaceName, Date, Distance, Laps)
VALUES
    ('Oslo Marathon', '2024-06-15', 42195, 1),
    ('Bergen City Marathon', '2024-04-22', 21097, 2),
    ('Trondheim Triathlon', '2024-08-12', 10000, 3),
    ('Stavanger Half Marathon', '2024-09-10', 21097, 1),
    ('Drammen Parkrun', '2024-05-14', 5000, 5),
    ('Arctic Race Tromsø', '2024-07-01', 10000, 4),
    ('Bodø Midnight Sun Run', '2024-06-21', 5000, 3),
    ('Arendal Waterfront Dash', '2024-08-28', 10000, 2),
    ('Oslo Fun Run', '2024-05-01', 3000, 3),
    ('Trondheim Winter Run', '2024-01-15', 5000, 5);

-- Generere eksempeldata for tabellen Registrations
INSERT INTO Registrations (MemberId, RaceId, RegistrationDate)
VALUES
    (1, 1, '2024-01-10'),
    (2, 1, '2024-02-15'),
    (3, 4, '2024-03-20'),
    (4, 4, '2024-04-10'),
    (5, 5, '2024-05-25'),
    (6, 6, '2024-06-01'),
    (7, 7, '2024-07-05'),
    (8, 8, '2024-08-20'),
    (9, 9, '2024-09-01'),
    (10, 10, '2024-10-15');

-- Generere eksempeldata for tabellen Results
INSERT INTO Results (MemberId, RaceId, Time)
VALUES
    (6, 1, '03:45:30'),
    (3, 1, '01:55:45'),
    (3, 4, '00:40:15'),
    (4, 4, '01:50:10'),
    (5, 5, '00:25:30'),
    (6, 6, '00:52:40'),
    (7, 7, '00:45:20'),
    (8, 8, '00:41:15'),
    (9, 9, '00:22:10'),
    (10, 10, '00:30:50');
