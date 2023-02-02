--
-- PostgreSQL database dump
--

-- Dumped from database version 14.5
-- Dumped by pg_dump version 14.5

-- Started on 2023-02-02 20:02:54

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

--
-- TOC entry 223 (class 1255 OID 99250)
-- Name: check_classroom(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.check_classroom(classroom_number character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
declare 
	classroom_id int4;
	begin
		if ((select count(1) from classrooms c 
    		where c."number" = classroom_number) = 0)
		then
			insert into classrooms (number) values(classroom_number) 
			RETURNING classrooms.id into strict classroom_id;
		else
			select c.id into strict classroom_id from classrooms c 
    		where c."number" = classroom_number;
		end if;
		
		return classroom_id;
	END;
$$;


ALTER FUNCTION public.check_classroom(classroom_number character varying) OWNER TO postgres;

--
-- TOC entry 229 (class 1255 OID 99252)
-- Name: check_employment(character varying, character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.check_employment(classroom_name character varying, group_title character varying, subject_name character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
declare 
	group_id int4;
	classroom_id int4;
	employment_id int4;
	begin
		group_id := check_group(group_title);
		classroom_id :=check_classroom(classroom_name);
	
		if ((SELECT count(1) FROM employments e
			where e.classroom = classroom_id
			and e."group" = group_id
			and e.subject = subject_name) = 0)
		then
			insert into employments (classroom, "group", subject)
				values (classroom_id, group_id, subject_name)
				RETURNING employments.id into strict employment_id;
		else
			SELECT  e.id into strict employment_id FROM employments e
			where e.classroom = classroom_id
			and e."group" = group_id
			and e.subject = subject_name;
		end if;
		return employment_id;
	END;
$$;


ALTER FUNCTION public.check_employment(classroom_name character varying, group_title character varying, subject_name character varying) OWNER TO postgres;

--
-- TOC entry 222 (class 1255 OID 99248)
-- Name: check_group(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.check_group(group_name character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
declare
	group_id int4;
	BEGIN
		if ((select count(1) from "groups" g 
    				where g.title = group_name) = 0) 
    	then
			insert into "groups" (title) values(group_name) 
			RETURNING "groups".id into strict group_id;
		else 
			select g.id into strict group_id 
			from "groups" g 
			where g.title  = group_name;
		end if;
		return group_id;
	END;
$$;


ALTER FUNCTION public.check_group(group_name character varying) OWNER TO postgres;

--
-- TOC entry 228 (class 1255 OID 99251)
-- Name: check_pair(date, integer, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.check_pair(date_current date, num_pair integer, teacher character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
declare 
	teacher_id int4;
	pair_id int4;
	BEGIN
		teacher_id := check_teacher(teacher);
		if ((select count(1) from pairs p
				where p."date" = date_current 
				and p.pair_number = num_pair 
				and p.teacher = teacher_id) = 0)
		then 
			insert into pairs("date", pair_number, teacher)
			values (date_current, num_pair, teacher_id)
			RETURNING pairs.id into strict pair_id;
		else
			select p.id into strict pair_id from pairs p
			where p."date" = date_current 
			and p.pair_number = num_pair 
			and p.teacher = teacher_id;
		end if;
		return pair_id;
	END;
$$;


ALTER FUNCTION public.check_pair(date_current date, num_pair integer, teacher character varying) OWNER TO postgres;

--
-- TOC entry 221 (class 1255 OID 99247)
-- Name: check_teacher(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.check_teacher(teacher character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
declare
	teacher_id int4;
	begin
		if ((select count(1) from teachers t
    				where t."name"  = teacher) = 0) then
			insert into teachers ("name") values(teacher) 
			RETURNING teachers.id into strict teacher_id;
		else 
			select t.id into strict teacher_id 
			from teachers t
			where t."name"  = teacher;
		end if;
		return teacher_id;
	END;
$$;


ALTER FUNCTION public.check_teacher(teacher character varying) OWNER TO postgres;

--
-- TOC entry 224 (class 1255 OID 99269)
-- Name: get_classroom(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_classroom(select_classroom character varying) RETURNS jsonb
    LANGUAGE plpgsql
    AS $$
	BEGIN
		return jsonb_agg(jsonb_build_object(
		'date', gt."date",
		'groups', jsonb_build_array(jsonb_build_object(
		'group', gt."group",
		'pairs', jsonb_build_array(jsonb_build_object(
			'pair_number', gt.pair_number ,
			'classroom', gt.classroom,
			'subject', gt.subject,
			'teacher', gt.teacher))
		))
		))
		from  get_timetable() gt
		where gt.classroom = select_classroom;
	END;
$$;


ALTER FUNCTION public.get_classroom(select_classroom character varying) OWNER TO postgres;

--
-- TOC entry 241 (class 1255 OID 99259)
-- Name: get_classroom_by_date(date, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_classroom_by_date(select_date date, select_classroom character varying) RETURNS jsonb
    LANGUAGE plpgsql
    AS $$
	begin
		return jsonb_agg(jsonb_build_object(
		'date', gt."date",
		'groups', jsonb_build_array(jsonb_build_object(
		'group', gt."group",
		'pairs', jsonb_build_array(jsonb_build_object(
			'pair_number', gt.pair_number ,
			'classroom', gt.classroom,
			'subject', gt.subject,
			'teacher', gt.teacher))
		))
		))
		from  get_timetable() gt
where gt."date" = select_date and gt.classroom = select_classroom;
	END;
$$;


ALTER FUNCTION public.get_classroom_by_date(select_date date, select_classroom character varying) OWNER TO postgres;

--
-- TOC entry 243 (class 1255 OID 99270)
-- Name: get_group(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_group(select_group character varying) RETURNS jsonb
    LANGUAGE plpgsql
    AS $$
	BEGIN
return jsonb_agg(jsonb_build_object(
		'date', gt."date",
		'groups', jsonb_build_array(jsonb_build_object(
		'group', gt."group",
		'pairs', jsonb_build_array(jsonb_build_object(
			'pair_number', gt.pair_number ,
			'classroom', gt.classroom,
			'subject', gt.subject,
			'teacher', gt.teacher))
		))
		))
		from  get_timetable() gt
		where gt."group" = select_group;
	END;
$$;


ALTER FUNCTION public.get_group(select_group character varying) OWNER TO postgres;

--
-- TOC entry 242 (class 1255 OID 99262)
-- Name: get_group_by_date(date, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_group_by_date(select_date date, select_group character varying) RETURNS jsonb
    LANGUAGE plpgsql
    AS $$
	BEGIN
		return jsonb_agg(jsonb_build_object(
		'date', gt."date",
		'groups', jsonb_build_array(jsonb_build_object(
		'group', gt."group",
		'pairs', jsonb_build_array(jsonb_build_object(
			'pair_number', gt.pair_number ,
			'classroom', gt.classroom,
			'subject', gt.subject,
			'teacher', gt.teacher))
		))
		))
		from  get_timetable() gt
where gt."date" = select_date and gt."group" = select_group;
	END;
$$;


ALTER FUNCTION public.get_group_by_date(select_date date, select_group character varying) OWNER TO postgres;

--
-- TOC entry 245 (class 1255 OID 99272)
-- Name: get_jsonb_timetable(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_jsonb_timetable() RETURNS jsonb
    LANGUAGE plpgsql
    AS $$
	begin
		return jsonb_agg(jsonb_build_object(
		'date', gt."date",
		'groups', jsonb_build_array(jsonb_build_object(
		'group', gt."group",
		'pairs', jsonb_build_array(jsonb_build_object(
			'pair_number', gt.pair_number ,
			'classroom', gt.classroom,
			'subject', gt.subject,
			'teacher', gt.teacher))
		))
		))
		from  get_timetable() gt;
	END;
$$;


ALTER FUNCTION public.get_jsonb_timetable() OWNER TO postgres;

--
-- TOC entry 246 (class 1255 OID 99273)
-- Name: get_jsonb_timetable_by_date(date); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_jsonb_timetable_by_date(select_date date) RETURNS jsonb
    LANGUAGE plpgsql
    AS $$
	begin
		return jsonb_agg(jsonb_build_object(
		'date', gt."date",
		'groups', jsonb_build_array(jsonb_build_object(
		'group', gt."group",
		'pairs', jsonb_build_array(jsonb_build_object(
			'pair_number', gt.pair_number ,
			'classroom', gt.classroom,
			'subject', gt.subject,
			'teacher', gt.teacher))
		))
		))
		from  get_timetable() gt
		where gt."date" = select_date;
	END;
$$;


ALTER FUNCTION public.get_jsonb_timetable_by_date(select_date date) OWNER TO postgres;

--
-- TOC entry 244 (class 1255 OID 99271)
-- Name: get_teacher(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_teacher(select_teacher character varying) RETURNS jsonb
    LANGUAGE plpgsql
    AS $$
	begin
		return jsonb_agg(jsonb_build_object(
		'date', gt."date",
		'groups', jsonb_build_array(jsonb_build_object(
		'group', gt."group",
		'pairs', jsonb_build_array(jsonb_build_object(
			'pair_number', gt.pair_number ,
			'classroom', gt.classroom,
			'subject', gt.subject,
			'teacher', gt.teacher))
		))
		))
		from  get_timetable() gt
		where gt.teacher = select_teacher;
	END;
$$;


ALTER FUNCTION public.get_teacher(select_teacher character varying) OWNER TO postgres;

--
-- TOC entry 239 (class 1255 OID 99261)
-- Name: get_teacher_by_date(date, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_teacher_by_date(select_date date, select_teacher character varying) RETURNS jsonb
    LANGUAGE plpgsql
    AS $$
	begin
		return jsonb_agg(jsonb_build_object(
		'date', gt."date",
		'groups', jsonb_build_array(jsonb_build_object(
		'group', gt."group",
		'pairs', jsonb_build_array(jsonb_build_object(
			'pair_number', gt.pair_number ,
			'classroom', gt.classroom,
			'subject', gt.subject,
			'teacher', gt.teacher))
		))
		))
		from  get_timetable() gt
		where gt."date" = select_date and gt.teacher = select_teacher;
	END;
$$;


ALTER FUNCTION public.get_teacher_by_date(select_date date, select_teacher character varying) OWNER TO postgres;

--
-- TOC entry 240 (class 1255 OID 99257)
-- Name: get_timetable(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_timetable() RETURNS TABLE(date date, pair_number smallint, "group" character varying, classroom character varying, subject character varying, teacher character varying)
    LANGUAGE plpgsql
    AS $$
	begin
		RETURN QUERY select 
		p."date",
		p.pair_number,
		g.title,
		c."number",
		e.subject,
		t."name" 
		from pair_employments pe
			left join employments e on pe.employment = e.id 
			left join pairs p on pe.pair  = p.id 
			left join classrooms c on e.classroom  = c.id 
			left join "groups" g  on e."group"  = g.id 
			left join teachers t on p.teacher = t.id;
	END;
$$;


ALTER FUNCTION public.get_timetable() OWNER TO postgres;

--
-- TOC entry 238 (class 1255 OID 99253)
-- Name: insert_pair_employment(date, integer, character varying, character varying, character varying, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.insert_pair_employment(IN date_current date, IN num_pair integer, IN teacher character varying, IN group_title character varying, IN classroom character varying, IN subject character varying)
    LANGUAGE plpgsql
    AS $$
declare 
	pair_id int4;
	employment_id int4;
	begin
		pair_id := check_pair(date_current, num_pair, teacher);
		employment_id := check_employment(classroom, group_title, subject);
	
		if ((select count(1) from pair_employments pe
			where pe.employment = employment_id
			and pe.pair = pair_id) = 0)
		then 
			insert into pair_employments (employment, pair) 
			values (employment_id, pair_id);
		else
			RAISE exception 'Такая пара уже есть';
		end if;
	END;
$$;


ALTER PROCEDURE public.insert_pair_employment(IN date_current date, IN num_pair integer, IN teacher character varying, IN group_title character varying, IN classroom character varying, IN subject character varying) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 214 (class 1259 OID 99195)
-- Name: classrooms; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.classrooms (
    id integer NOT NULL,
    number character varying(20) NOT NULL
);


ALTER TABLE public.classrooms OWNER TO postgres;

--
-- TOC entry 213 (class 1259 OID 99194)
-- Name: classroom_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.classroom_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.classroom_id_seq OWNER TO postgres;

--
-- TOC entry 3371 (class 0 OID 0)
-- Dependencies: 213
-- Name: classroom_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.classroom_id_seq OWNED BY public.classrooms.id;


--
-- TOC entry 218 (class 1259 OID 99214)
-- Name: employments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.employments (
    id integer NOT NULL,
    classroom integer NOT NULL,
    "group" integer NOT NULL,
    subject character varying(20)
);


ALTER TABLE public.employments OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 99213)
-- Name: employment_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.employment_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.employment_id_seq OWNER TO postgres;

--
-- TOC entry 3372 (class 0 OID 0)
-- Dependencies: 217
-- Name: employment_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employment_id_seq OWNED BY public.employments.id;


--
-- TOC entry 210 (class 1259 OID 99181)
-- Name: groups; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.groups (
    id integer NOT NULL,
    title character varying(20) NOT NULL
);


ALTER TABLE public.groups OWNER TO postgres;

--
-- TOC entry 209 (class 1259 OID 99180)
-- Name: groups_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.groups_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.groups_id_seq OWNER TO postgres;

--
-- TOC entry 3373 (class 0 OID 0)
-- Dependencies: 209
-- Name: groups_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.groups_id_seq OWNED BY public.groups.id;


--
-- TOC entry 220 (class 1259 OID 99231)
-- Name: pair_employments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.pair_employments (
    pair integer NOT NULL,
    employment integer NOT NULL,
    id integer NOT NULL
);


ALTER TABLE public.pair_employments OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 99230)
-- Name: pair_employments_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.pair_employments_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.pair_employments_id_seq OWNER TO postgres;

--
-- TOC entry 3374 (class 0 OID 0)
-- Dependencies: 219
-- Name: pair_employments_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.pair_employments_id_seq OWNED BY public.pair_employments.id;


--
-- TOC entry 216 (class 1259 OID 99202)
-- Name: pairs; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.pairs (
    id integer NOT NULL,
    date date NOT NULL,
    pair_number smallint NOT NULL,
    teacher integer NOT NULL
);


ALTER TABLE public.pairs OWNER TO postgres;

--
-- TOC entry 215 (class 1259 OID 99201)
-- Name: pairs_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.pairs_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.pairs_id_seq OWNER TO postgres;

--
-- TOC entry 3375 (class 0 OID 0)
-- Dependencies: 215
-- Name: pairs_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.pairs_id_seq OWNED BY public.pairs.id;


--
-- TOC entry 212 (class 1259 OID 99188)
-- Name: teachers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.teachers (
    id integer NOT NULL,
    name character varying(50) NOT NULL
);


ALTER TABLE public.teachers OWNER TO postgres;

--
-- TOC entry 211 (class 1259 OID 99187)
-- Name: teachers_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.teachers_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.teachers_id_seq OWNER TO postgres;

--
-- TOC entry 3376 (class 0 OID 0)
-- Dependencies: 211
-- Name: teachers_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.teachers_id_seq OWNED BY public.teachers.id;


--
-- TOC entry 3206 (class 2604 OID 99198)
-- Name: classrooms id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.classrooms ALTER COLUMN id SET DEFAULT nextval('public.classroom_id_seq'::regclass);


--
-- TOC entry 3208 (class 2604 OID 99217)
-- Name: employments id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employments ALTER COLUMN id SET DEFAULT nextval('public.employment_id_seq'::regclass);


--
-- TOC entry 3204 (class 2604 OID 99184)
-- Name: groups id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.groups ALTER COLUMN id SET DEFAULT nextval('public.groups_id_seq'::regclass);


--
-- TOC entry 3209 (class 2604 OID 99234)
-- Name: pair_employments id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.pair_employments ALTER COLUMN id SET DEFAULT nextval('public.pair_employments_id_seq'::regclass);


--
-- TOC entry 3207 (class 2604 OID 99205)
-- Name: pairs id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.pairs ALTER COLUMN id SET DEFAULT nextval('public.pairs_id_seq'::regclass);


--
-- TOC entry 3205 (class 2604 OID 99191)
-- Name: teachers id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.teachers ALTER COLUMN id SET DEFAULT nextval('public.teachers_id_seq'::regclass);


--
-- TOC entry 3215 (class 2606 OID 99200)
-- Name: classrooms classroom_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.classrooms
    ADD CONSTRAINT classroom_pk PRIMARY KEY (id);


--
-- TOC entry 3219 (class 2606 OID 99219)
-- Name: employments employment_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employments
    ADD CONSTRAINT employment_pk PRIMARY KEY (id);


--
-- TOC entry 3211 (class 2606 OID 99186)
-- Name: groups groups_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.groups
    ADD CONSTRAINT groups_pk PRIMARY KEY (id);


--
-- TOC entry 3221 (class 2606 OID 99236)
-- Name: pair_employments pair_employments_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.pair_employments
    ADD CONSTRAINT pair_employments_pk PRIMARY KEY (id);


--
-- TOC entry 3217 (class 2606 OID 99207)
-- Name: pairs pairs_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.pairs
    ADD CONSTRAINT pairs_pk PRIMARY KEY (id);


--
-- TOC entry 3213 (class 2606 OID 99193)
-- Name: teachers teachers_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.teachers
    ADD CONSTRAINT teachers_pk PRIMARY KEY (id);


--
-- TOC entry 3223 (class 2606 OID 99220)
-- Name: employments employment_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employments
    ADD CONSTRAINT employment_fk FOREIGN KEY ("group") REFERENCES public.groups(id);


--
-- TOC entry 3224 (class 2606 OID 99225)
-- Name: employments employment_fk_1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employments
    ADD CONSTRAINT employment_fk_1 FOREIGN KEY (classroom) REFERENCES public.classrooms(id);


--
-- TOC entry 3225 (class 2606 OID 99237)
-- Name: pair_employments pair_employments_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.pair_employments
    ADD CONSTRAINT pair_employments_fk FOREIGN KEY (pair) REFERENCES public.pairs(id);


--
-- TOC entry 3226 (class 2606 OID 99242)
-- Name: pair_employments pair_employments_fk_1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.pair_employments
    ADD CONSTRAINT pair_employments_fk_1 FOREIGN KEY (employment) REFERENCES public.employments(id);


--
-- TOC entry 3222 (class 2606 OID 99208)
-- Name: pairs pairs_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.pairs
    ADD CONSTRAINT pairs_fk FOREIGN KEY (teacher) REFERENCES public.teachers(id);


-- Completed on 2023-02-02 20:02:54

--
-- PostgreSQL database dump complete
--

