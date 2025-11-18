CREATE TABLE IF NOT EXISTS products (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100),
    price NUMERIC(10,2),
    amount INT DEFAULT 0
);

INSERT INTO products (name, price, amount) VALUES
('Laptop', 999.99, 10),
('Mouse', 25.50, 50),
('Keyboard', 45.00, 30),
('Monitor', 150.75, 20),
('Printer', 120.00, 15),
('Desk Chair', 85.20, 25),
('USB Drive', 15.00, 100),
('Webcam', 70.00, 40),
('Headphones', 60.00, 35),
('Smartphone', 699.99, 12);
