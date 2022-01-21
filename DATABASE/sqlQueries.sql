-- creating database
create database helperland_database

-- Table 1
-- (Contact us):
CREATE TABLE Contact
(
	contact_id int NOT NULL PRIMARY KEY,
	firstname varchar(50),
	lastname varchar(50),
	mobile bigint,
	email varchar(50),
	subject varchar(50),
	message varchar(50)
);


-- Table 2
-- (user_detail):
CREATE TABLE User_detail
(
	userdetail_id int NOT NULL PRIMARY KEY,
	firstname varchar(50),
	lastname varchar(50),
	email varchar(50),
	mobile bigint,
	password varchar(50),
	birthdate date,
	language varchar(50),
	user_role int
);


-- Table 3
-- (service_provider):
CREATE TABLE service_provider
(
	service_provider int NOT NULL PRIMARY KEY,
	userdetail_id int FOREIGN KEY REFERENCES User_detail(userdetail_id),
	pincode int,
	nationality varchar(50),
	gender varchar(50),
	avtar varchar(100)
);


-- Table 4
-- (customer_address):
CREATE TABLE customer_address
(
	customer_address_id int NOT NULL PRIMARY KEY,
	userdetail_id int FOREIGN KEY REFERENCES User_detail(userdetail_id),
	street_name varchar(50),
	house_number int,
	pincode int,
	city varchar(50),
	mobile bigint
);


-- Table 5
-- (customer_order):
CREATE TABLE customer_order
(
	customer_order_id int NOT NULL PRIMARY KEY,
	customerdetail_id int FOREIGN KEY REFERENCES User_detail(userdetail_id),
	serviceproviderdetail_id int FOREIGN KEY REFERENCES User_detail(userdetail_id),
	customer_address_id int FOREIGN KEY REFERENCES customer_address(customer_address_id),
	order_date date,
	start_time time,
	basic_service varchar(50),
	extra_service int,
	comments varchar(50),
	pets bit,
);


-- Table 6
-- (FAVOURITE_SERVICE_PROVIDER):
CREATE TABLE favourite_service_provider
(
	favourite_service_provider_id int NOT NULL PRIMARY KEY,
	customerdetails_id int FOREIGN KEY REFERENCES User_detail(userdetail_id),
	serviceproviderdetails_id int FOREIGN KEY REFERENCES User_detail(userdetail_id),
);



-- Table 7
-- (BLOCK_SERVICE_PROVIDER):
CREATE TABLE block_service_provider
(
	block_service_provider_id int NOT NULL PRIMARY KEY,
	customerdetails_id int FOREIGN KEY REFERENCES User_detail(userdetail_id),
	serviceproviderdetails_id int FOREIGN KEY REFERENCES User_detail(userdetail_id),
);


-- Table 8
-- (extra_services):
CREATE TABLE extra_services
(
	extra_services_id int,
	customer_order_id int FOREIGN KEY REFERENCES customer_order(customer_order_id),
	service_inside_cabinat bit,
	service_fridge bit,
	sercice_oven bit,
	sercice_laundry bit,
	sercice_window bit
);