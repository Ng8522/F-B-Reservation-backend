-- Users Table (Admins, Staff, and Customers)
CREATE TABLE IF NOT EXISTS users (
    id CHAR(36) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    contact_number VARCHAR(20) UNIQUE NULL,
    email VARCHAR(100) UNIQUE NULL,
    password VARCHAR(255) NULL,
    role ENUM('Admin', 'Outlet_Staff', 'Customer') NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by CHAR(36) NULL,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    updated_by CHAR(36) NULL
);

-- Outlets Table
CREATE TABLE IF NOT EXISTS outlets (
    id CHAR(36) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    location TEXT NOT NULL,
    operating_hours VARCHAR(50) NOT NULL,
    capacity INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by CHAR(36) NULL,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    updated_by CHAR(36) NULL
);

-- Staff Table (Mapping staff to specific outlets)
CREATE TABLE IF NOT EXISTS staff (
    id CHAR(36) PRIMARY KEY,
    user_id CHAR(36) NOT NULL,
    outlet_id CHAR(36) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by CHAR(36) NULL,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    updated_by CHAR(36) NULL,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (outlet_id) REFERENCES outlets(id) ON DELETE CASCADE
);

-- Reservations Table
CREATE TABLE IF NOT EXISTS reservations (
    id CHAR(36) PRIMARY KEY,
    customer_id CHAR(36) NOT NULL,
    outlet_id CHAR(36) NOT NULL,
    reservation_date DATE NOT NULL,
    reservation_time TIME NOT NULL,
    num_guests INT NOT NULL,
    status ENUM('Pending', 'Confirmed', 'Cancelled', 'Completed') DEFAULT 'Pending',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by CHAR(36) NULL,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    updated_by CHAR(36) NULL,
    FOREIGN KEY (customer_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (outlet_id) REFERENCES outlets(id) ON DELETE CASCADE
);

-- Queue Management Table
CREATE TABLE IF NOT EXISTS queues (
    id CHAR(36) PRIMARY KEY,
    customer_id CHAR(36) NOT NULL,
    outlet_id CHAR(36) NOT NULL,
    num_guests INT NOT NULL,
    special_requests TEXT NULL,
    queue_position INT NOT NULL,
    status ENUM('Waiting', 'Called', 'Cancelled') DEFAULT 'Waiting',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by CHAR(36) NULL,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    updated_by CHAR(36) NULL,
    FOREIGN KEY (customer_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (outlet_id) REFERENCES outlets(id) ON DELETE CASCADE
);

-- Blacklist Table (For banned customer numbers)
CREATE TABLE IF NOT EXISTS blacklist (
    id CHAR(36) PRIMARY KEY,
    contact_number VARCHAR(20) UNIQUE NOT NULL,
    reason TEXT NOT NULL,
    banned_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by CHAR(36) NULL
);

-- Notifications Table (For WhatsApp notifications)
CREATE TABLE IF NOT EXISTS notifications (
    id CHAR(36) PRIMARY KEY,
    user_id CHAR(36) NOT NULL,
    message TEXT NOT NULL,
    status ENUM('Pending', 'Sent', 'Failed') DEFAULT 'Pending',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by CHAR(36) NULL,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    updated_by CHAR(36) NULL,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Indexing for Optimization
CREATE INDEX IF NOT EXISTS idx_reservation_outlet_date ON reservations (outlet_id, reservation_date);
CREATE INDEX IF NOT EXISTS idx_queue_outlet ON queues (outlet_id, queue_position);
