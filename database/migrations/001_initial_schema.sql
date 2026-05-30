--
-- PostgreSQL database dump
--

\restrict 0a0L4ONm0Fi24YiHDPV4AZW2qhsJUavl7OBI6yNpqRBk5iFUpDeQB9nSrXE34ga

-- Dumped from database version 17.10
-- Dumped by pg_dump version 17.10

-- Started on 2026-05-30 14:04:40

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 2 (class 3079 OID 16389)
-- Name: uuid-ossp; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;


--
-- TOC entry 5024 (class 0 OID 0)
-- Dependencies: 2
-- Name: EXTENSION "uuid-ossp"; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';


--
-- TOC entry 235 (class 1255 OID 16567)
-- Name: fn_set_updated_at(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_set_updated_at() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.fn_set_updated_at() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 220 (class 1259 OID 16433)
-- Name: driver_profiles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.driver_profiles (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
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
    status character varying(20) DEFAULT 'PENDING'::character varying NOT NULL,
    avg_rating numeric(3,2) DEFAULT 0.00 NOT NULL,
    total_trips integer DEFAULT 0 NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT driver_profiles_status_check CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'PENDING'::character varying, 'SUSPENDED'::character varying, 'INACTIVE'::character varying])::text[]))),
    CONSTRAINT driver_profiles_vehicle_type_check CHECK (((vehicle_type)::text = ANY ((ARRAY['PICKUP'::character varying, 'VAN'::character varying, 'TRUCK_SMALL'::character varying, 'TRUCK_MEDIUM'::character varying, 'TRUCK_LARGE'::character varying, 'FLATBED'::character varying])::text[])))
);


ALTER TABLE public.driver_profiles OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 16481)
-- Name: fare_offers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.fare_offers (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    shipment_id uuid NOT NULL,
    driver_id uuid NOT NULL,
    offered_price numeric(10,2) NOT NULL,
    currency character(3) DEFAULT 'PEN'::bpchar NOT NULL,
    status character varying(20) DEFAULT 'PENDING'::character varying NOT NULL,
    driver_note character varying(300),
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT fare_offers_offered_price_check CHECK ((offered_price > (0)::numeric)),
    CONSTRAINT fare_offers_status_check CHECK (((status)::text = ANY ((ARRAY['PENDING'::character varying, 'ACCEPTED'::character varying, 'REJECTED'::character varying, 'WITHDRAWN'::character varying, 'EXPIRED'::character varying])::text[])))
);


ALTER TABLE public.fare_offers OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 16540)
-- Name: ratings; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ratings (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    trip_id uuid NOT NULL,
    rated_by_user_id uuid NOT NULL,
    rated_user_id uuid NOT NULL,
    score smallint NOT NULL,
    comment character varying(500),
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT ratings_score_check CHECK (((score >= 1) AND (score <= 5)))
);


ALTER TABLE public.ratings OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 16417)
-- Name: refresh_tokens; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.refresh_tokens (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    user_id uuid NOT NULL,
    token_hash text NOT NULL,
    expires_at timestamp with time zone NOT NULL,
    revoked_at timestamp with time zone,
    created_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.refresh_tokens OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 16456)
-- Name: shipment_requests; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.shipment_requests (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
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
    currency character(3) DEFAULT 'PEN'::bpchar NOT NULL,
    status character varying(20) DEFAULT 'OPEN'::character varying NOT NULL,
    cancel_reason text,
    expires_at timestamp with time zone DEFAULT (now() + '02:00:00'::interval) NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    deleted_at timestamp with time zone,
    CONSTRAINT shipment_requests_status_check CHECK (((status)::text = ANY ((ARRAY['OPEN'::character varying, 'NEGOTIATING'::character varying, 'ACCEPTED'::character varying, 'CANCELLED'::character varying, 'EXPIRED'::character varying])::text[]))),
    CONSTRAINT shipment_requests_weight_kg_check CHECK ((weight_kg > (0)::numeric))
);


ALTER TABLE public.shipment_requests OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 16506)
-- Name: trips; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.trips (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    shipment_id uuid NOT NULL,
    driver_id uuid NOT NULL,
    accepted_offer_id uuid NOT NULL,
    final_price numeric(10,2) NOT NULL,
    currency character(3) DEFAULT 'PEN'::bpchar NOT NULL,
    payment_method character varying(30) DEFAULT 'CASH_ON_DELIVERY'::character varying NOT NULL,
    status character varying(20) DEFAULT 'CONFIRMED'::character varying NOT NULL,
    cancel_reason text,
    started_at timestamp with time zone,
    completed_at timestamp with time zone,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT trips_final_price_check CHECK ((final_price > (0)::numeric)),
    CONSTRAINT trips_payment_method_check CHECK (((payment_method)::text = ANY ((ARRAY['CASH_ON_DELIVERY'::character varying, 'TRANSFER'::character varying, 'WALLET'::character varying])::text[]))),
    CONSTRAINT trips_status_check CHECK (((status)::text = ANY ((ARRAY['CONFIRMED'::character varying, 'IN_PROGRESS'::character varying, 'COMPLETED'::character varying, 'CANCELLED'::character varying])::text[])))
);


ALTER TABLE public.trips OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 16400)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    full_name character varying(120) NOT NULL,
    email character varying(180) NOT NULL,
    phone character varying(20) NOT NULL,
    password_hash text NOT NULL,
    role character varying(20) DEFAULT 'CLIENT'::character varying NOT NULL,
    status character varying(20) DEFAULT 'PENDING_VERIFICATION'::character varying NOT NULL,
    avatar_url text,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    deleted_at timestamp with time zone,
    CONSTRAINT users_role_check CHECK (((role)::text = ANY ((ARRAY['CLIENT'::character varying, 'DRIVER'::character varying, 'BOTH'::character varying, 'ADMIN'::character varying])::text[]))),
    CONSTRAINT users_status_check CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'PENDING_VERIFICATION'::character varying, 'SUSPENDED'::character varying, 'DELETED'::character varying])::text[])))
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 4833 (class 2606 OID 16445)
-- Name: driver_profiles driver_profiles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.driver_profiles
    ADD CONSTRAINT driver_profiles_pkey PRIMARY KEY (id);


--
-- TOC entry 4845 (class 2606 OID 16492)
-- Name: fare_offers fare_offers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fare_offers
    ADD CONSTRAINT fare_offers_pkey PRIMARY KEY (id);


--
-- TOC entry 4856 (class 2606 OID 16549)
-- Name: ratings ratings_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ratings
    ADD CONSTRAINT ratings_pkey PRIMARY KEY (id);


--
-- TOC entry 4831 (class 2606 OID 16425)
-- Name: refresh_tokens refresh_tokens_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.refresh_tokens
    ADD CONSTRAINT refresh_tokens_pkey PRIMARY KEY (id);


--
-- TOC entry 4843 (class 2606 OID 16472)
-- Name: shipment_requests shipment_requests_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shipment_requests
    ADD CONSTRAINT shipment_requests_pkey PRIMARY KEY (id);


--
-- TOC entry 4852 (class 2606 OID 16521)
-- Name: trips trips_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.trips
    ADD CONSTRAINT trips_pkey PRIMARY KEY (id);


--
-- TOC entry 4827 (class 2606 OID 16413)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- TOC entry 4834 (class 1259 OID 16454)
-- Name: idx_driver_status; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_driver_status ON public.driver_profiles USING btree (status);


--
-- TOC entry 4835 (class 1259 OID 16455)
-- Name: idx_driver_vehicle_type; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_driver_vehicle_type ON public.driver_profiles USING btree (vehicle_type, status);


--
-- TOC entry 4846 (class 1259 OID 16505)
-- Name: idx_fo_driver_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_fo_driver_id ON public.fare_offers USING btree (driver_id);


--
-- TOC entry 4847 (class 1259 OID 16504)
-- Name: idx_fo_shipment_status; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_fo_shipment_status ON public.fare_offers USING btree (shipment_id, status);


--
-- TOC entry 4854 (class 1259 OID 16566)
-- Name: idx_rating_rated_user; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_rating_rated_user ON public.ratings USING btree (rated_user_id);


--
-- TOC entry 4828 (class 1259 OID 16432)
-- Name: idx_rt_token_hash; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_rt_token_hash ON public.refresh_tokens USING btree (token_hash);


--
-- TOC entry 4829 (class 1259 OID 16431)
-- Name: idx_rt_user_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_rt_user_id ON public.refresh_tokens USING btree (user_id);


--
-- TOC entry 4839 (class 1259 OID 16478)
-- Name: idx_sr_client_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_sr_client_id ON public.shipment_requests USING btree (client_id);


--
-- TOC entry 4840 (class 1259 OID 16480)
-- Name: idx_sr_expires_at; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_sr_expires_at ON public.shipment_requests USING btree (expires_at) WHERE ((status)::text = 'OPEN'::text);


--
-- TOC entry 4841 (class 1259 OID 16479)
-- Name: idx_sr_status_created; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_sr_status_created ON public.shipment_requests USING btree (status, created_at DESC) WHERE (deleted_at IS NULL);


--
-- TOC entry 4849 (class 1259 OID 16538)
-- Name: idx_trip_driver_status; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_trip_driver_status ON public.trips USING btree (driver_id, status);


--
-- TOC entry 4850 (class 1259 OID 16539)
-- Name: idx_trip_status; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_trip_status ON public.trips USING btree (status);


--
-- TOC entry 4823 (class 1259 OID 16416)
-- Name: idx_users_role_status; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_users_role_status ON public.users USING btree (role, status);


--
-- TOC entry 4836 (class 1259 OID 16453)
-- Name: uq_driver_license; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX uq_driver_license ON public.driver_profiles USING btree (license_number);


--
-- TOC entry 4837 (class 1259 OID 16452)
-- Name: uq_driver_plate; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX uq_driver_plate ON public.driver_profiles USING btree (vehicle_plate);


--
-- TOC entry 4838 (class 1259 OID 16451)
-- Name: uq_driver_user; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX uq_driver_user ON public.driver_profiles USING btree (user_id);


--
-- TOC entry 4848 (class 1259 OID 16503)
-- Name: uq_fo_one_pending_per_driver; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX uq_fo_one_pending_per_driver ON public.fare_offers USING btree (shipment_id, driver_id) WHERE ((status)::text = 'PENDING'::text);


--
-- TOC entry 4857 (class 1259 OID 16565)
-- Name: uq_rating_per_user_per_trip; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX uq_rating_per_user_per_trip ON public.ratings USING btree (trip_id, rated_by_user_id);


--
-- TOC entry 4853 (class 1259 OID 16537)
-- Name: uq_trip_shipment; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX uq_trip_shipment ON public.trips USING btree (shipment_id);


--
-- TOC entry 4824 (class 1259 OID 16414)
-- Name: uq_users_email; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX uq_users_email ON public.users USING btree (email) WHERE (deleted_at IS NULL);


--
-- TOC entry 4825 (class 1259 OID 16415)
-- Name: uq_users_phone; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX uq_users_phone ON public.users USING btree (phone) WHERE (deleted_at IS NULL);


--
-- TOC entry 4870 (class 2620 OID 16569)
-- Name: driver_profiles trg_driver_profiles_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_driver_profiles_updated_at BEFORE UPDATE ON public.driver_profiles FOR EACH ROW EXECUTE FUNCTION public.fn_set_updated_at();


--
-- TOC entry 4872 (class 2620 OID 16571)
-- Name: fare_offers trg_fare_offers_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_fare_offers_updated_at BEFORE UPDATE ON public.fare_offers FOR EACH ROW EXECUTE FUNCTION public.fn_set_updated_at();


--
-- TOC entry 4871 (class 2620 OID 16570)
-- Name: shipment_requests trg_shipment_requests_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_shipment_requests_updated_at BEFORE UPDATE ON public.shipment_requests FOR EACH ROW EXECUTE FUNCTION public.fn_set_updated_at();


--
-- TOC entry 4873 (class 2620 OID 16572)
-- Name: trips trg_trips_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_trips_updated_at BEFORE UPDATE ON public.trips FOR EACH ROW EXECUTE FUNCTION public.fn_set_updated_at();


--
-- TOC entry 4869 (class 2620 OID 16568)
-- Name: users trg_users_updated_at; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_users_updated_at BEFORE UPDATE ON public.users FOR EACH ROW EXECUTE FUNCTION public.fn_set_updated_at();


--
-- TOC entry 4859 (class 2606 OID 16446)
-- Name: driver_profiles driver_profiles_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.driver_profiles
    ADD CONSTRAINT driver_profiles_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- TOC entry 4861 (class 2606 OID 16498)
-- Name: fare_offers fare_offers_driver_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fare_offers
    ADD CONSTRAINT fare_offers_driver_id_fkey FOREIGN KEY (driver_id) REFERENCES public.users(id);


--
-- TOC entry 4862 (class 2606 OID 16493)
-- Name: fare_offers fare_offers_shipment_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fare_offers
    ADD CONSTRAINT fare_offers_shipment_id_fkey FOREIGN KEY (shipment_id) REFERENCES public.shipment_requests(id) ON DELETE CASCADE;


--
-- TOC entry 4866 (class 2606 OID 16555)
-- Name: ratings ratings_rated_by_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ratings
    ADD CONSTRAINT ratings_rated_by_user_id_fkey FOREIGN KEY (rated_by_user_id) REFERENCES public.users(id);


--
-- TOC entry 4867 (class 2606 OID 16560)
-- Name: ratings ratings_rated_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ratings
    ADD CONSTRAINT ratings_rated_user_id_fkey FOREIGN KEY (rated_user_id) REFERENCES public.users(id);


--
-- TOC entry 4868 (class 2606 OID 16550)
-- Name: ratings ratings_trip_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ratings
    ADD CONSTRAINT ratings_trip_id_fkey FOREIGN KEY (trip_id) REFERENCES public.trips(id) ON DELETE CASCADE;


--
-- TOC entry 4858 (class 2606 OID 16426)
-- Name: refresh_tokens refresh_tokens_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.refresh_tokens
    ADD CONSTRAINT refresh_tokens_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- TOC entry 4860 (class 2606 OID 16473)
-- Name: shipment_requests shipment_requests_client_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shipment_requests
    ADD CONSTRAINT shipment_requests_client_id_fkey FOREIGN KEY (client_id) REFERENCES public.users(id);


--
-- TOC entry 4863 (class 2606 OID 16532)
-- Name: trips trips_accepted_offer_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.trips
    ADD CONSTRAINT trips_accepted_offer_id_fkey FOREIGN KEY (accepted_offer_id) REFERENCES public.fare_offers(id);


--
-- TOC entry 4864 (class 2606 OID 16527)
-- Name: trips trips_driver_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.trips
    ADD CONSTRAINT trips_driver_id_fkey FOREIGN KEY (driver_id) REFERENCES public.users(id);


--
-- TOC entry 4865 (class 2606 OID 16522)
-- Name: trips trips_shipment_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.trips
    ADD CONSTRAINT trips_shipment_id_fkey FOREIGN KEY (shipment_id) REFERENCES public.shipment_requests(id);


-- Completed on 2026-05-30 14:04:41

--
-- PostgreSQL database dump complete
--

\unrestrict 0a0L4ONm0Fi24YiHDPV4AZW2qhsJUavl7OBI6yNpqRBk5iFUpDeQB9nSrXE34ga

