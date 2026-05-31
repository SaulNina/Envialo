-- Corregido para Supabase: uuid_generate_v4() → gen_random_uuid()

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA extensions;

CREATE OR REPLACE FUNCTION public.fn_set_updated_at() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$;

SET default_tablespace = '';
SET default_table_access_method = heap;

CREATE TABLE public.users (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    full_name character varying(120) NOT NULL,
    email character varying(180) NOT NULL,
    phone character varying(20) NOT NULL,
    password_hash text NOT NULL,
    role character varying(20) DEFAULT 'CLIENT' NOT NULL,
    status character varying(20) DEFAULT 'PENDING_VERIFICATION' NOT NULL,
    avatar_url text,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    deleted_at timestamp with time zone,
    CONSTRAINT users_role_check CHECK (((role)::text = ANY ((ARRAY['CLIENT'::character varying, 'DRIVER'::character varying, 'BOTH'::character varying, 'ADMIN'::character varying])::text[]))),
    CONSTRAINT users_status_check CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'PENDING_VERIFICATION'::character varying, 'SUSPENDED'::character varying, 'DELETED'::character varying])::text[])))
);

CREATE TABLE public.refresh_tokens (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    token_hash text NOT NULL,
    expires_at timestamp with time zone NOT NULL,
    revoked_at timestamp with time zone,
    created_at timestamp with time zone DEFAULT now() NOT NULL
);

CREATE TABLE public.driver_profiles (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    license_number character varying(30) NOT NULL,
    license_expiry date NOT NULL,
    vehicle_type character varying(40) NOT NULL,
    vehicle_plate character varying(15) NOT NULL,
    vehicle_brand character varying(60),
    vehicle_model character varying(60),
    vehicle_year smallint,
    payload_capacity_kg numeric(8,2) NOT NULL,
    volume_capacity_m3 numeric(6,2),
    status character varying(20) DEFAULT 'PENDING' NOT NULL,
    avg_rating numeric(3,2) DEFAULT 0.00 NOT NULL,
    total_trips integer DEFAULT 0 NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT driver_profiles_status_check CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'PENDING'::character varying, 'SUSPENDED'::character varying, 'INACTIVE'::character varying])::text[]))),
    CONSTRAINT driver_profiles_vehicle_type_check CHECK (((vehicle_type)::text = ANY ((ARRAY['PICKUP'::character varying, 'VAN'::character varying, 'TRUCK_SMALL'::character varying, 'TRUCK_MEDIUM'::character varying, 'TRUCK_LARGE'::character varying, 'FLATBED'::character varying])::text[])))
);

CREATE TABLE public.shipment_requests (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    client_id uuid NOT NULL,
    origin_address text NOT NULL,
    origin_reference character varying(200),
    destination_address text NOT NULL,
    dest_reference character varying(200),
    cargo_description text NOT NULL,
    weight_kg numeric(8,2) NOT NULL,
    width_cm numeric(6,2),
    height_cm numeric(6,2),
    depth_cm numeric(6,2),
    requires_refrigeration boolean DEFAULT false NOT NULL,
    fragile boolean DEFAULT false NOT NULL,
    photo_url text,
    suggested_price numeric(10,2),
    currency character(3) DEFAULT 'PEN' NOT NULL,
    status character varying(20) DEFAULT 'OPEN' NOT NULL,
    cancel_reason text,
    expires_at timestamp with time zone DEFAULT (now() + '02:00:00'::interval) NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    deleted_at timestamp with time zone,
    CONSTRAINT shipment_requests_status_check CHECK (((status)::text = ANY ((ARRAY['OPEN'::character varying, 'NEGOTIATING'::character varying, 'ACCEPTED'::character varying, 'CANCELLED'::character varying, 'EXPIRED'::character varying])::text[]))),
    CONSTRAINT shipment_requests_weight_kg_check CHECK ((weight_kg > (0)::numeric))
);

CREATE TABLE public.fare_offers (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    shipment_id uuid NOT NULL,
    driver_id uuid NOT NULL,
    offered_price numeric(10,2) NOT NULL,
    currency character(3) DEFAULT 'PEN' NOT NULL,
    status character varying(20) DEFAULT 'PENDING' NOT NULL,
    driver_note character varying(300),
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT fare_offers_offered_price_check CHECK ((offered_price > (0)::numeric)),
    CONSTRAINT fare_offers_status_check CHECK (((status)::text = ANY ((ARRAY['PENDING'::character varying, 'ACCEPTED'::character varying, 'REJECTED'::character varying, 'WITHDRAWN'::character varying, 'EXPIRED'::character varying])::text[])))
);

CREATE TABLE public.trips (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    shipment_id uuid NOT NULL,
    driver_id uuid NOT NULL,
    accepted_offer_id uuid NOT NULL,
    final_price numeric(10,2) NOT NULL,
    currency character(3) DEFAULT 'PEN' NOT NULL,
    payment_method character varying(30) DEFAULT 'CASH_ON_DELIVERY' NOT NULL,
    status character varying(20) DEFAULT 'CONFIRMED' NOT NULL,
    cancel_reason text,
    started_at timestamp with time zone,
    completed_at timestamp with time zone,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT trips_final_price_check CHECK ((final_price > (0)::numeric)),
    CONSTRAINT trips_payment_method_check CHECK (((payment_method)::text = ANY ((ARRAY['CASH_ON_DELIVERY'::character varying, 'TRANSFER'::character varying, 'WALLET'::character varying])::text[]))),
    CONSTRAINT trips_status_check CHECK (((status)::text = ANY ((ARRAY['CONFIRMED'::character varying, 'IN_PROGRESS'::character varying, 'COMPLETED'::character varying, 'CANCELLED'::character varying])::text[])))
);

CREATE TABLE public.ratings (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    trip_id uuid NOT NULL,
    rated_by_user_id uuid NOT NULL,
    rated_user_id uuid NOT NULL,
    score smallint NOT NULL,
    comment character varying(500),
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT ratings_score_check CHECK (((score >= 1) AND (score <= 5)))
);

-- Primary keys
ALTER TABLE ONLY public.users ADD CONSTRAINT users_pkey PRIMARY KEY (id);
ALTER TABLE ONLY public.refresh_tokens ADD CONSTRAINT refresh_tokens_pkey PRIMARY KEY (id);
ALTER TABLE ONLY public.driver_profiles ADD CONSTRAINT driver_profiles_pkey PRIMARY KEY (id);
ALTER TABLE ONLY public.shipment_requests ADD CONSTRAINT shipment_requests_pkey PRIMARY KEY (id);
ALTER TABLE ONLY public.fare_offers ADD CONSTRAINT fare_offers_pkey PRIMARY KEY (id);
ALTER TABLE ONLY public.trips ADD CONSTRAINT trips_pkey PRIMARY KEY (id);
ALTER TABLE ONLY public.ratings ADD CONSTRAINT ratings_pkey PRIMARY KEY (id);

-- Indexes
CREATE UNIQUE INDEX uq_users_email ON public.users USING btree (email) WHERE (deleted_at IS NULL);
CREATE UNIQUE INDEX uq_users_phone ON public.users USING btree (phone) WHERE (deleted_at IS NULL);
CREATE INDEX idx_users_role_status ON public.users USING btree (role, status);
CREATE INDEX idx_rt_user_id ON public.refresh_tokens USING btree (user_id);
CREATE INDEX idx_rt_token_hash ON public.refresh_tokens USING btree (token_hash);
CREATE UNIQUE INDEX uq_driver_user ON public.driver_profiles USING btree (user_id);
CREATE UNIQUE INDEX uq_driver_license ON public.driver_profiles USING btree (license_number);
CREATE UNIQUE INDEX uq_driver_plate ON public.driver_profiles USING btree (vehicle_plate);
CREATE INDEX idx_driver_status ON public.driver_profiles USING btree (status);
CREATE INDEX idx_driver_vehicle_type ON public.driver_profiles USING btree (vehicle_type, status);
CREATE INDEX idx_sr_client_id ON public.shipment_requests USING btree (client_id);
CREATE INDEX idx_sr_status_created ON public.shipment_requests USING btree (status, created_at DESC) WHERE (deleted_at IS NULL);
CREATE INDEX idx_sr_expires_at ON public.shipment_requests USING btree (expires_at) WHERE ((status)::text = 'OPEN'::text);
CREATE INDEX idx_fo_shipment_status ON public.fare_offers USING btree (shipment_id, status);
CREATE INDEX idx_fo_driver_id ON public.fare_offers USING btree (driver_id);
CREATE UNIQUE INDEX uq_fo_one_pending_per_driver ON public.fare_offers USING btree (shipment_id, driver_id) WHERE ((status)::text = 'PENDING'::text);
CREATE UNIQUE INDEX uq_trip_shipment ON public.trips USING btree (shipment_id);
CREATE INDEX idx_trip_driver_status ON public.trips USING btree (driver_id, status);
CREATE INDEX idx_trip_status ON public.trips USING btree (status);
CREATE UNIQUE INDEX uq_rating_per_user_per_trip ON public.ratings USING btree (trip_id, rated_by_user_id);
CREATE INDEX idx_rating_rated_user ON public.ratings USING btree (rated_user_id);

-- Triggers
CREATE TRIGGER trg_users_updated_at BEFORE UPDATE ON public.users FOR EACH ROW EXECUTE FUNCTION public.fn_set_updated_at();
CREATE TRIGGER trg_driver_profiles_updated_at BEFORE UPDATE ON public.driver_profiles FOR EACH ROW EXECUTE FUNCTION public.fn_set_updated_at();
CREATE TRIGGER trg_shipment_requests_updated_at BEFORE UPDATE ON public.shipment_requests FOR EACH ROW EXECUTE FUNCTION public.fn_set_updated_at();
CREATE TRIGGER trg_fare_offers_updated_at BEFORE UPDATE ON public.fare_offers FOR EACH ROW EXECUTE FUNCTION public.fn_set_updated_at();
CREATE TRIGGER trg_trips_updated_at BEFORE UPDATE ON public.trips FOR EACH ROW EXECUTE FUNCTION public.fn_set_updated_at();

-- Foreign keys
ALTER TABLE ONLY public.refresh_tokens ADD CONSTRAINT refresh_tokens_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;
ALTER TABLE ONLY public.driver_profiles ADD CONSTRAINT driver_profiles_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;
ALTER TABLE ONLY public.shipment_requests ADD CONSTRAINT shipment_requests_client_id_fkey FOREIGN KEY (client_id) REFERENCES public.users(id);
ALTER TABLE ONLY public.fare_offers ADD CONSTRAINT fare_offers_shipment_id_fkey FOREIGN KEY (shipment_id) REFERENCES public.shipment_requests(id) ON DELETE CASCADE;
ALTER TABLE ONLY public.fare_offers ADD CONSTRAINT fare_offers_driver_id_fkey FOREIGN KEY (driver_id) REFERENCES public.users(id);
ALTER TABLE ONLY public.trips ADD CONSTRAINT trips_shipment_id_fkey FOREIGN KEY (shipment_id) REFERENCES public.shipment_requests(id);
ALTER TABLE ONLY public.trips ADD CONSTRAINT trips_driver_id_fkey FOREIGN KEY (driver_id) REFERENCES public.users(id);
ALTER TABLE ONLY public.trips ADD CONSTRAINT trips_accepted_offer_id_fkey FOREIGN KEY (accepted_offer_id) REFERENCES public.fare_offers(id);
ALTER TABLE ONLY public.ratings ADD CONSTRAINT ratings_trip_id_fkey FOREIGN KEY (trip_id) REFERENCES public.trips(id) ON DELETE CASCADE;
ALTER TABLE ONLY public.ratings ADD CONSTRAINT ratings_rated_by_user_id_fkey FOREIGN KEY (rated_by_user_id) REFERENCES public.users(id);
ALTER TABLE ONLY public.ratings ADD CONSTRAINT ratings_rated_user_id_fkey FOREIGN KEY (rated_user_id) REFERENCES public.users(id);
