-- Database: transportes_orellana
CREATE DATABASE transportes_orellana
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.UTF-8'
    LC_CTYPE = 'en_US.UTF-8'
    LOCALE_PROVIDER = 'libc'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1
    IS_TEMPLATE = False;

CREATE TABLE cliente(
    cliente_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    correo VARCHAR(254) NOT NULL,
    telefono VARCHAR(15) NOT NULL,
    direccion VARCHAR(500) NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'activo',
    fecha_registro TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_cliente_estado 
        CHECK (estado IN ('activo', 'inactivo'))
);

CREATE TABLE motorista(
    motorista_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    direccion VARCHAR(500) NOT NULL,
    correo VARCHAR(254) NOT NULL,
    telefono VARCHAR(15) NOT NULL,
    perfil_social VARCHAR(2048),
    curriculum VARCHAR(2048),
    estado VARCHAR(20) NOT NULL DEFAULT 'activo',
    fecha_registro TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_motorista_estado 
        CHECK (estado IN ('activo', 'inactivo'))
);

CREATE TABLE unidad_transporte(
    unidad_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    placa CHAR(8) NOT NULL,
    marca VARCHAR(25) NOT NULL,
    capacidad_toneladas NUMERIC(6,2) NOT NULL,
    anio_fabricacion SMALLINT NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'activo',
    fecha_registro TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_unidad_estado 
        CHECK (estado IN ('activo', 'inactivo'))
);

CREATE TABLE flete(
    flete_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    cliente_id INT NOT NULL,
    unidad_id INT NOT NULL,
    motorista_id INT NOT NULL,
    descripcion TEXT,
    estado VARCHAR(20) NOT NULL DEFAULT 'programado',
    monto_cobro NUMERIC(10,2) NOT NULL,
    lugar_recolecta VARCHAR(200) NOT NULL,
    lugar_entrega VARCHAR(200) NOT NULL,
    hora_salida TIMESTAMP NOT NULL,
    hora_destino TIMESTAMP NOT NULL,
    consideraciones TEXT,
    fecha_registro TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_flete_estado 
        CHECK (estado IN ('programado', 'en_proceso', 'terminado', 'con_devolucion', 'con_queja')),
    CONSTRAINT fk_cliente
        FOREIGN KEY(cliente_id) REFERENCES cliente(cliente_id),
    CONSTRAINT fk_unidad
        FOREIGN KEY(unidad_id) REFERENCES unidad_transporte(unidad_id),
    CONSTRAINT fk_motorista
        FOREIGN KEY(motorista_id) REFERENCES motorista(motorista_id)
);

CREATE TABLE gasto_flete(
    gasto_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    flete_id INT NOT NULL,
    tipo_gasto VARCHAR(20) NOT NULL,
    concepto VARCHAR(100) NOT NULL,
    monto NUMERIC(10,2) NOT NULL,
    fecha_registro TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_tipo_gasto 
        CHECK (tipo_gasto IN ('combustible', 'peaje', 'alimentacion', 'hospedaje', 'mantenimiento', 'otro')),
    CONSTRAINT fk_flete
        FOREIGN KEY(flete_id) REFERENCES flete(flete_id)
);