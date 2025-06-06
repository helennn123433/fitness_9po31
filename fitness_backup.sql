PGDMP      (                }            fitness    16.3    16.3 }               0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            �           1262    66628    fitness    DATABASE     {   CREATE DATABASE fitness WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Russian_Russia.1251';
    DROP DATABASE fitness;
                postgres    false                        3079    66629    pgcrypto 	   EXTENSION     <   CREATE EXTENSION IF NOT EXISTS pgcrypto WITH SCHEMA public;
    DROP EXTENSION pgcrypto;
                   false            �           0    0    EXTENSION pgcrypto    COMMENT     <   COMMENT ON EXTENSION pgcrypto IS 'cryptographic functions';
                        false    2            &           1255    66666 �   add_trainer(character varying, character varying, character varying, character varying, character varying, integer, character varying, character varying, character varying, integer)    FUNCTION       CREATE FUNCTION public.add_trainer(p_first_name character varying, p_last_name character varying, p_middle_name character varying, p_email character varying, p_phone_number character varying, p_age_user integer, p_login character varying, p_password character varying, p_education character varying, p_experience_years integer) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_role_id INTEGER;
    v_user_id INTEGER;
BEGIN
    -- 1. Получить ID роли "тренер"
    SELECT id INTO v_role_id
    FROM role
    WHERE name = 'тренер'
    LIMIT 1;

    IF v_role_id IS NULL THEN
        RAISE EXCEPTION 'Role "тренер" not found';
    END IF;

    -- 2. Вставить нового пользователя
    INSERT INTO users (
        name_user,
        lastname_user,
        middle_name,
        email,
        phone_number,
        role_id,
        age_user,
        login,
        password
    ) VALUES (
        p_first_name,
        p_last_name,
        p_middle_name,
        p_email,
        p_phone_number,
        v_role_id,
        p_age_user,
        p_login,
        p_password
    )
    RETURNING user_id INTO v_user_id;

    -- 3. Вставить запись в таблицу trainer
    INSERT INTO trainer (
        trainer_education,
        trainer_experiance,
        user_id
    ) VALUES (
        p_education,
        p_experience_years,
        v_user_id
    );

    RETURN TRUE;
EXCEPTION
    WHEN OTHERS THEN
        RAISE NOTICE 'Error occurred: %', SQLERRM;
        RETURN FALSE;
END;
$$;
 G  DROP FUNCTION public.add_trainer(p_first_name character varying, p_last_name character varying, p_middle_name character varying, p_email character varying, p_phone_number character varying, p_age_user integer, p_login character varying, p_password character varying, p_education character varying, p_experience_years integer);
       public          postgres    false            '           1255    66667    calculate_abonement_end_date()    FUNCTION     Y  CREATE FUNCTION public.calculate_abonement_end_date() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_abonement_duration INTEGER;
BEGIN
    -- Получаем продолжительность абонемента из таблицы abonement
    SELECT abonement_long INTO v_abonement_duration
    FROM abonement
    WHERE id_abonement = NEW.id_abonement;
    
    -- Если абонемент не найден, генерируем ошибку
    IF v_abonement_duration IS NULL THEN
        RAISE EXCEPTION 'Абонемент с ID % не найден', NEW.id_abonement;
    END IF;
    
    -- Рассчитываем дату окончания (дата начала + продолжительность в днях)
    NEW.date_end := NEW.date_start + (v_abonement_duration * INTERVAL '1 day');
    
    RETURN NEW;
END;
$$;
 5   DROP FUNCTION public.calculate_abonement_end_date();
       public          postgres    false            (           1255    66668 #   get_abonement_id(character varying)    FUNCTION     N  CREATE FUNCTION public.get_abonement_id(p_abonement_name character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_abonement_id INTEGER;
BEGIN
    -- Получаем id абонемента по его имени
    SELECT id_abonement INTO v_abonement_id
    FROM abonement
    WHERE abonement_name = p_abonement_name
    LIMIT 1;

    -- Если абонемент не найден, возвращаем NULL
    IF v_abonement_id IS NULL THEN
        RAISE EXCEPTION 'Abonement "%" not found', p_abonement_name;
    END IF;

    RETURN v_abonement_id;
END;
$$;
 K   DROP FUNCTION public.get_abonement_id(p_abonement_name character varying);
       public          postgres    false            )           1255    66669 �   register_client_with_abonement(character varying, character varying, character varying, character varying, character varying, integer, character varying, character varying, character varying, date) 	   PROCEDURE     �  CREATE PROCEDURE public.register_client_with_abonement(IN p_first_name character varying, IN p_last_name character varying, IN p_middle_name character varying, IN p_email character varying, IN p_phone character varying, IN p_age integer, IN p_login character varying, IN p_password character varying, IN p_abonement_name character varying, IN p_start_date date, OUT p_success boolean)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_role_id INTEGER;
    v_abonement_id INTEGER;
    v_user_id INTEGER;
    v_client_id INTEGER;
BEGIN
    p_success := FALSE;

    -- Получаем ID роли "клиент"
    SELECT id INTO v_role_id FROM role WHERE name = 'клиент';
    IF v_role_id IS NULL THEN
        RAISE EXCEPTION 'Role "клиент" not found';
    END IF;
    
    BEGIN
        v_abonement_id := get_abonement_id(p_abonement_name);
    EXCEPTION WHEN OTHERS THEN
        RAISE EXCEPTION 'Abonement "%" not found', p_abonement_name;
    END;

    -- Основная транзакция
    BEGIN
        -- Создаем пользователя
        INSERT INTO users (
            name_user, lastname_user, middle_name, email,
            phone_number, role_id, age_user, login, password
        ) VALUES (
            p_first_name, p_last_name, p_middle_name, p_email,
            p_phone, v_role_id, p_age, p_login, p_password
        ) RETURNING user_id INTO v_user_id;

        -- Создаем клиента
        INSERT INTO client (user_id)
        VALUES (v_user_id) RETURNING id_client INTO v_client_id;
        
        -- Привязываем абонемент (дата окончания рассчитается триггером)
        INSERT INTO abonement_client (
            date_start, id_client, id_abonement
        ) VALUES (
            p_start_date, v_client_id, v_abonement_id
        );

        p_success := TRUE;
    EXCEPTION
        WHEN OTHERS THEN
            RAISE EXCEPTION 'Error during insert: %', SQLERRM;
            ROLLBACK;
    END;
END;
$$;
 �  DROP PROCEDURE public.register_client_with_abonement(IN p_first_name character varying, IN p_last_name character varying, IN p_middle_name character varying, IN p_email character varying, IN p_phone character varying, IN p_age integer, IN p_login character varying, IN p_password character varying, IN p_abonement_name character varying, IN p_start_date date, OUT p_success boolean);
       public          postgres    false            *           1255    66670 �   update_client_with_abonement(character varying, character varying, character varying, character varying, character varying, character varying, integer, character varying, date) 	   PROCEDURE     �  CREATE PROCEDURE public.update_client_with_abonement(IN p_login character varying, IN p_first_name character varying, IN p_last_name character varying, IN p_middle_name character varying, IN p_email character varying, IN p_phone character varying, IN p_age integer, IN p_abonement_name character varying, IN p_start_date date, OUT p_success boolean)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_user_id INTEGER;
    v_client_id INTEGER;
    v_abonement_id INTEGER;
BEGIN
    p_success := FALSE;

    -- Ищем пользователя по логину
    SELECT user_id INTO v_user_id FROM users WHERE login = p_login;
    IF v_user_id IS NULL THEN
        RETURN;
    END IF;

    -- Обновляем данные пользователя (Логин и Пароль НЕ трогаем!)
    UPDATE users
    SET 
        name_user = p_first_name,
        lastname_user = p_last_name,
        middle_name = p_middle_name,
        email = p_email,
        phone_number = p_phone,
        age_user = p_age
    WHERE user_id = v_user_id;

    -- Ищем клиента по user_id
    SELECT id_client INTO v_client_id FROM client WHERE user_id = v_user_id;
    IF v_client_id IS NULL THEN
        RETURN;
    END IF;

    -- Получаем ID абонемента
    BEGIN
        v_abonement_id := get_abonement_id(p_abonement_name);
    EXCEPTION WHEN OTHERS THEN
        RETURN;
    END;

    -- Обновляем или создаем запись абонемента
    IF EXISTS (SELECT 1 FROM abonement_client WHERE id_client = v_client_id) THEN
        UPDATE abonement_client
        SET 
            id_abonement = v_abonement_id,
            date_start = p_start_date
        WHERE id_client = v_client_id;
    ELSE
        INSERT INTO abonement_client (date_start, id_client, id_abonement)
        VALUES (p_start_date, v_client_id, v_abonement_id);
    END IF;

    p_success := TRUE;
END;
$$;
 ]  DROP PROCEDURE public.update_client_with_abonement(IN p_login character varying, IN p_first_name character varying, IN p_last_name character varying, IN p_middle_name character varying, IN p_email character varying, IN p_phone character varying, IN p_age integer, IN p_abonement_name character varying, IN p_start_date date, OUT p_success boolean);
       public          postgres    false            +           1255    66671 �   update_trainer(integer, character varying, character varying, character varying, character varying, character varying, integer, character varying, integer)    FUNCTION     L  CREATE FUNCTION public.update_trainer(p_id_trainer integer, p_first_name character varying, p_last_name character varying, p_middle_name character varying, p_email character varying, p_phone_number character varying, p_age_user integer, p_education character varying, p_experience_years integer) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_user_id INTEGER;
BEGIN
    -- Найдём user_id, связанный с тренером
    SELECT user_id INTO v_user_id
    FROM trainer
    WHERE id_trainer = p_id_trainer;

    IF v_user_id IS NULL THEN
        RETURN FALSE;
    END IF;

    -- Обновим trainer
    UPDATE trainer
    SET 
        trainer_education = p_education,
        trainer_experiance = p_experience_years
    WHERE id_trainer = p_id_trainer;

    -- Обновим users
    UPDATE users
    SET
        name_user = p_first_name,
        lastname_user = p_last_name,
        middle_name = p_middle_name,
        email = p_email,
        phone_number = p_phone_number,
        age_user = p_age_user
    WHERE user_id = v_user_id;

    RETURN TRUE;
END;
$$;
 '  DROP FUNCTION public.update_trainer(p_id_trainer integer, p_first_name character varying, p_last_name character varying, p_middle_name character varying, p_email character varying, p_phone_number character varying, p_age_user integer, p_education character varying, p_experience_years integer);
       public          postgres    false            �            1259    66672 	   abonement    TABLE     
  CREATE TABLE public.abonement (
    id_abonement integer NOT NULL,
    abonement_name character varying(20) NOT NULL,
    abonement_price integer NOT NULL,
    abonement_long integer NOT NULL,
    abonement_description text NOT NULL,
    abonement_freeze integer
);
    DROP TABLE public.abonement;
       public         heap    postgres    false            �            1259    66677    abonement_client    TABLE     �   CREATE TABLE public.abonement_client (
    id_abonement_client integer NOT NULL,
    date_start date NOT NULL,
    date_end date NOT NULL,
    id_client integer NOT NULL,
    id_abonement integer NOT NULL
);
 $   DROP TABLE public.abonement_client;
       public         heap    postgres    false            �            1259    66680 (   abonement_client_id_abonement_client_seq    SEQUENCE     �   CREATE SEQUENCE public.abonement_client_id_abonement_client_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 ?   DROP SEQUENCE public.abonement_client_id_abonement_client_seq;
       public          postgres    false    217            �           0    0 (   abonement_client_id_abonement_client_seq    SEQUENCE OWNED BY     u   ALTER SEQUENCE public.abonement_client_id_abonement_client_seq OWNED BY public.abonement_client.id_abonement_client;
          public          postgres    false    218            �            1259    66681    abonement_id_abonement_seq    SEQUENCE     �   CREATE SEQUENCE public.abonement_id_abonement_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 1   DROP SEQUENCE public.abonement_id_abonement_seq;
       public          postgres    false    216            �           0    0    abonement_id_abonement_seq    SEQUENCE OWNED BY     Y   ALTER SEQUENCE public.abonement_id_abonement_seq OWNED BY public.abonement.id_abonement;
          public          postgres    false    219            �            1259    66682    additional_services    TABLE     �   CREATE TABLE public.additional_services (
    id_services integer NOT NULL,
    services_name character varying(40),
    services_price integer NOT NULL,
    services_description text NOT NULL,
    image_path character varying
);
 '   DROP TABLE public.additional_services;
       public         heap    postgres    false            �            1259    66687 #   additional_services_id_services_seq    SEQUENCE     �   CREATE SEQUENCE public.additional_services_id_services_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 :   DROP SEQUENCE public.additional_services_id_services_seq;
       public          postgres    false    220            �           0    0 #   additional_services_id_services_seq    SEQUENCE OWNED BY     k   ALTER SEQUENCE public.additional_services_id_services_seq OWNED BY public.additional_services.id_services;
          public          postgres    false    221            �            1259    66688    client    TABLE     T   CREATE TABLE public.client (
    id_client integer NOT NULL,
    user_id integer
);
    DROP TABLE public.client;
       public         heap    postgres    false            �            1259    66691    client_id_client_seq    SEQUENCE     �   CREATE SEQUENCE public.client_id_client_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 +   DROP SEQUENCE public.client_id_client_seq;
       public          postgres    false    222            �           0    0    client_id_client_seq    SEQUENCE OWNED BY     M   ALTER SEQUENCE public.client_id_client_seq OWNED BY public.client.id_client;
          public          postgres    false    223            �            1259    66692    users    TABLE     �  CREATE TABLE public.users (
    user_id integer NOT NULL,
    name_user character varying(40) NOT NULL,
    lastname_user character varying(60) NOT NULL,
    middle_name character varying(60),
    email character varying(100) NOT NULL,
    phone_number character varying(15),
    role_id integer,
    age_user integer,
    login character varying(255),
    password character varying(255)
);
    DROP TABLE public.users;
       public         heap    postgres    false            �            1259    66697    client_info_view    VIEW     �  CREATE VIEW public.client_info_view AS
 SELECT c.id_client AS client_id,
    u.name_user AS first_name,
    u.lastname_user AS last_name,
    u.middle_name,
    u.email,
    u.phone_number,
    u.age_user AS age,
    a.abonement_name,
    ac.id_abonement_client,
    ac.date_start AS abonement_start_date,
    ac.date_end AS abonement_end_date,
        CASE
            WHEN (ac.date_end < CURRENT_DATE) THEN 'Истек'::text
            WHEN (ac.date_start > CURRENT_DATE) THEN 'Не начался'::text
            ELSE 'Активен'::text
        END AS abonement_status,
    GREATEST((ac.date_end - CURRENT_DATE), 0) AS days_remaining,
    u.login,
    u.password
   FROM (((public.client c
     JOIN public.users u ON ((c.user_id = u.user_id)))
     LEFT JOIN public.abonement_client ac ON ((c.id_client = ac.id_client)))
     LEFT JOIN public.abonement a ON ((ac.id_abonement = a.id_abonement)));
 #   DROP VIEW public.client_info_view;
       public          postgres    false    216    216    217    217    217    217    217    222    222    224    224    224    224    224    224    224    224    224            �            1259    66702    hall    TABLE     �   CREATE TABLE public.hall (
    id_hall integer NOT NULL,
    capacity_hall integer NOT NULL,
    id_type_hall integer NOT NULL,
    area integer,
    is_active boolean,
    image_path character varying
);
    DROP TABLE public.hall;
       public         heap    postgres    false            �            1259    66707 	   hall_type    TABLE     �   CREATE TABLE public.hall_type (
    id_type_hall integer NOT NULL,
    name_type_hall character varying(30) NOT NULL,
    type_hall_desc text NOT NULL
);
    DROP TABLE public.hall_type;
       public         heap    postgres    false            �            1259    66712    hall_details    VIEW     !  CREATE VIEW public.hall_details AS
 SELECT h.id_hall,
    h.capacity_hall AS capacity,
    h.area,
    h.is_active,
    ht.name_type_hall AS hall_type,
    ht.type_hall_desc AS type_description
   FROM (public.hall h
     JOIN public.hall_type ht ON ((h.id_type_hall = ht.id_type_hall)));
    DROP VIEW public.hall_details;
       public          postgres    false    226    226    226    226    227    227    227    226            �            1259    66716    hall_id_hall_seq    SEQUENCE     �   CREATE SEQUENCE public.hall_id_hall_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 '   DROP SEQUENCE public.hall_id_hall_seq;
       public          postgres    false    226            �           0    0    hall_id_hall_seq    SEQUENCE OWNED BY     E   ALTER SEQUENCE public.hall_id_hall_seq OWNED BY public.hall.id_hall;
          public          postgres    false    229            �            1259    66717    hall_type_id_type_hall_seq    SEQUENCE     �   CREATE SEQUENCE public.hall_type_id_type_hall_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 1   DROP SEQUENCE public.hall_type_id_type_hall_seq;
       public          postgres    false    227            �           0    0    hall_type_id_type_hall_seq    SEQUENCE OWNED BY     Y   ALTER SEQUENCE public.hall_type_id_type_hall_seq OWNED BY public.hall_type.id_type_hall;
          public          postgres    false    230            �            1259    66718    lesson    TABLE     /  CREATE TABLE public.lesson (
    id_lesson integer NOT NULL,
    id_hall integer NOT NULL,
    lesson_date date NOT NULL,
    id_trainer integer NOT NULL,
    id_type_lesson integer NOT NULL,
    id_abonement_client integer,
    start_time time without time zone,
    end_time time without time zone
);
    DROP TABLE public.lesson;
       public         heap    postgres    false            �            1259    66721    lesson_id_lesson_seq    SEQUENCE     �   CREATE SEQUENCE public.lesson_id_lesson_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 +   DROP SEQUENCE public.lesson_id_lesson_seq;
       public          postgres    false    231            �           0    0    lesson_id_lesson_seq    SEQUENCE OWNED BY     M   ALTER SEQUENCE public.lesson_id_lesson_seq OWNED BY public.lesson.id_lesson;
          public          postgres    false    232            �            1259    66722    lesson_type    TABLE     �   CREATE TABLE public.lesson_type (
    id_type_lesson integer NOT NULL,
    type_lesson_desc text NOT NULL,
    type_lesson_name character varying(255) NOT NULL
);
    DROP TABLE public.lesson_type;
       public         heap    postgres    false            �            1259    66727    trainer    TABLE     �   CREATE TABLE public.trainer (
    id_trainer integer NOT NULL,
    trainer_education character varying(60) NOT NULL,
    trainer_experiance integer NOT NULL,
    user_id integer
);
    DROP TABLE public.trainer;
       public         heap    postgres    false            �            1259    66730    lesson_schedule_view    VIEW     �  CREATE VIEW public.lesson_schedule_view AS
 SELECT l.id_lesson,
    ht.name_type_hall AS type_hall_name,
    l.lesson_date,
    l.start_time AS lesson_start,
    l.end_time AS lesson_end,
    concat(u.lastname_user, ' ', u.name_user) AS trainer_full_name,
    l.id_abonement_client AS id_abonement,
    lt.type_lesson_name AS lesson_type_name
   FROM (((((public.lesson l
     JOIN public.trainer t ON ((l.id_trainer = t.id_trainer)))
     JOIN public.users u ON ((t.user_id = u.user_id)))
     JOIN public.lesson_type lt ON ((l.id_type_lesson = lt.id_type_lesson)))
     JOIN public.hall h ON ((l.id_hall = h.id_hall)))
     JOIN public.hall_type ht ON ((h.id_type_hall = ht.id_type_hall)))
  ORDER BY l.lesson_date, l.start_time;
 '   DROP VIEW public.lesson_schedule_view;
       public          postgres    false    231    231    231    231    231    227    227    226    226    224    224    224    234    234    233    233    231    231    231            �            1259    66735    lesson_type_id_type_lesson_seq    SEQUENCE     �   CREATE SEQUENCE public.lesson_type_id_type_lesson_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 5   DROP SEQUENCE public.lesson_type_id_type_lesson_seq;
       public          postgres    false    233            �           0    0    lesson_type_id_type_lesson_seq    SEQUENCE OWNED BY     a   ALTER SEQUENCE public.lesson_type_id_type_lesson_seq OWNED BY public.lesson_type.id_type_lesson;
          public          postgres    false    236            �            1259    66736    role    TABLE     _   CREATE TABLE public.role (
    id integer NOT NULL,
    name character varying(50) NOT NULL
);
    DROP TABLE public.role;
       public         heap    postgres    false            �            1259    66739    role_id_seq    SEQUENCE     �   CREATE SEQUENCE public.role_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 "   DROP SEQUENCE public.role_id_seq;
       public          postgres    false    237            �           0    0    role_id_seq    SEQUENCE OWNED BY     ;   ALTER SEQUENCE public.role_id_seq OWNED BY public.role.id;
          public          postgres    false    238            �            1259    66740    trainer_details    VIEW     �  CREATE VIEW public.trainer_details AS
 SELECT t.id_trainer,
    u.name_user AS first_name,
    u.lastname_user AS last_name,
    u.middle_name,
    u.email,
    u.phone_number,
    u.age_user,
    t.trainer_education AS education,
    t.trainer_experiance AS experience_years,
    u.login,
    u.password
   FROM (public.trainer t
     JOIN public.users u ON ((t.user_id = u.user_id)));
 "   DROP VIEW public.trainer_details;
       public          postgres    false    234    224    224    224    224    224    224    224    224    224    234    234    234            �            1259    66744    trainer_id_trainer_seq    SEQUENCE     �   CREATE SEQUENCE public.trainer_id_trainer_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 -   DROP SEQUENCE public.trainer_id_trainer_seq;
       public          postgres    false    234            �           0    0    trainer_id_trainer_seq    SEQUENCE OWNED BY     Q   ALTER SEQUENCE public.trainer_id_trainer_seq OWNED BY public.trainer.id_trainer;
          public          postgres    false    240            �            1259    66745    user_with_role    VIEW     1  CREATE VIEW public.user_with_role AS
 SELECT u.user_id,
    u.name_user AS first_name,
    u.lastname_user AS last_name,
    u.middle_name,
    u.email,
    u.phone_number,
    u.age_user,
    u.login,
    r.name AS role_name
   FROM (public.users u
     LEFT JOIN public.role r ON ((u.role_id = r.id)));
 !   DROP VIEW public.user_with_role;
       public          postgres    false    224    237    237    224    224    224    224    224    224    224    224            �            1259    66749    users_user_id_seq    SEQUENCE     �   CREATE SEQUENCE public.users_user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 (   DROP SEQUENCE public.users_user_id_seq;
       public          postgres    false    224            �           0    0    users_user_id_seq    SEQUENCE OWNED BY     G   ALTER SEQUENCE public.users_user_id_seq OWNED BY public.users.user_id;
          public          postgres    false    242            �            1259    66750    visit    TABLE     �   CREATE TABLE public.visit (
    id_visit integer NOT NULL,
    date_visit date NOT NULL,
    date_time integer NOT NULL,
    id_abonement_client integer NOT NULL
);
    DROP TABLE public.visit;
       public         heap    postgres    false            �            1259    66753    visit_id_visit_seq    SEQUENCE     �   CREATE SEQUENCE public.visit_id_visit_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 )   DROP SEQUENCE public.visit_id_visit_seq;
       public          postgres    false    243            �           0    0    visit_id_visit_seq    SEQUENCE OWNED BY     I   ALTER SEQUENCE public.visit_id_visit_seq OWNED BY public.visit.id_visit;
          public          postgres    false    244            �            1259    66754    visit_services    TABLE     �   CREATE TABLE public.visit_services (
    id_visit_services integer NOT NULL,
    id_visit integer NOT NULL,
    id_time integer NOT NULL,
    id_services integer NOT NULL
);
 "   DROP TABLE public.visit_services;
       public         heap    postgres    false            �            1259    66757 $   visit_services_id_visit_services_seq    SEQUENCE     �   CREATE SEQUENCE public.visit_services_id_visit_services_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 ;   DROP SEQUENCE public.visit_services_id_visit_services_seq;
       public          postgres    false    245            �           0    0 $   visit_services_id_visit_services_seq    SEQUENCE OWNED BY     m   ALTER SEQUENCE public.visit_services_id_visit_services_seq OWNED BY public.visit_services.id_visit_services;
          public          postgres    false    246            �           2604    66758    abonement id_abonement    DEFAULT     �   ALTER TABLE ONLY public.abonement ALTER COLUMN id_abonement SET DEFAULT nextval('public.abonement_id_abonement_seq'::regclass);
 E   ALTER TABLE public.abonement ALTER COLUMN id_abonement DROP DEFAULT;
       public          postgres    false    219    216            �           2604    66759 $   abonement_client id_abonement_client    DEFAULT     �   ALTER TABLE ONLY public.abonement_client ALTER COLUMN id_abonement_client SET DEFAULT nextval('public.abonement_client_id_abonement_client_seq'::regclass);
 S   ALTER TABLE public.abonement_client ALTER COLUMN id_abonement_client DROP DEFAULT;
       public          postgres    false    218    217            �           2604    66760    additional_services id_services    DEFAULT     �   ALTER TABLE ONLY public.additional_services ALTER COLUMN id_services SET DEFAULT nextval('public.additional_services_id_services_seq'::regclass);
 N   ALTER TABLE public.additional_services ALTER COLUMN id_services DROP DEFAULT;
       public          postgres    false    221    220            �           2604    66761    client id_client    DEFAULT     t   ALTER TABLE ONLY public.client ALTER COLUMN id_client SET DEFAULT nextval('public.client_id_client_seq'::regclass);
 ?   ALTER TABLE public.client ALTER COLUMN id_client DROP DEFAULT;
       public          postgres    false    223    222            �           2604    66762    hall id_hall    DEFAULT     l   ALTER TABLE ONLY public.hall ALTER COLUMN id_hall SET DEFAULT nextval('public.hall_id_hall_seq'::regclass);
 ;   ALTER TABLE public.hall ALTER COLUMN id_hall DROP DEFAULT;
       public          postgres    false    229    226            �           2604    66763    hall_type id_type_hall    DEFAULT     �   ALTER TABLE ONLY public.hall_type ALTER COLUMN id_type_hall SET DEFAULT nextval('public.hall_type_id_type_hall_seq'::regclass);
 E   ALTER TABLE public.hall_type ALTER COLUMN id_type_hall DROP DEFAULT;
       public          postgres    false    230    227            �           2604    66764    lesson id_lesson    DEFAULT     t   ALTER TABLE ONLY public.lesson ALTER COLUMN id_lesson SET DEFAULT nextval('public.lesson_id_lesson_seq'::regclass);
 ?   ALTER TABLE public.lesson ALTER COLUMN id_lesson DROP DEFAULT;
       public          postgres    false    232    231            �           2604    66765    lesson_type id_type_lesson    DEFAULT     �   ALTER TABLE ONLY public.lesson_type ALTER COLUMN id_type_lesson SET DEFAULT nextval('public.lesson_type_id_type_lesson_seq'::regclass);
 I   ALTER TABLE public.lesson_type ALTER COLUMN id_type_lesson DROP DEFAULT;
       public          postgres    false    236    233            �           2604    66766    role id    DEFAULT     b   ALTER TABLE ONLY public.role ALTER COLUMN id SET DEFAULT nextval('public.role_id_seq'::regclass);
 6   ALTER TABLE public.role ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    238    237            �           2604    66767    trainer id_trainer    DEFAULT     x   ALTER TABLE ONLY public.trainer ALTER COLUMN id_trainer SET DEFAULT nextval('public.trainer_id_trainer_seq'::regclass);
 A   ALTER TABLE public.trainer ALTER COLUMN id_trainer DROP DEFAULT;
       public          postgres    false    240    234            �           2604    66768    users user_id    DEFAULT     n   ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.users_user_id_seq'::regclass);
 <   ALTER TABLE public.users ALTER COLUMN user_id DROP DEFAULT;
       public          postgres    false    242    224            �           2604    66769    visit id_visit    DEFAULT     p   ALTER TABLE ONLY public.visit ALTER COLUMN id_visit SET DEFAULT nextval('public.visit_id_visit_seq'::regclass);
 =   ALTER TABLE public.visit ALTER COLUMN id_visit DROP DEFAULT;
       public          postgres    false    244    243            �           2604    66770     visit_services id_visit_services    DEFAULT     �   ALTER TABLE ONLY public.visit_services ALTER COLUMN id_visit_services SET DEFAULT nextval('public.visit_services_id_visit_services_seq'::regclass);
 O   ALTER TABLE public.visit_services ALTER COLUMN id_visit_services DROP DEFAULT;
       public          postgres    false    246    245            c          0    66672 	   abonement 
   TABLE DATA           �   COPY public.abonement (id_abonement, abonement_name, abonement_price, abonement_long, abonement_description, abonement_freeze) FROM stdin;
    public          postgres    false    216   G�       d          0    66677    abonement_client 
   TABLE DATA           n   COPY public.abonement_client (id_abonement_client, date_start, date_end, id_client, id_abonement) FROM stdin;
    public          postgres    false    217   ��       g          0    66682    additional_services 
   TABLE DATA           {   COPY public.additional_services (id_services, services_name, services_price, services_description, image_path) FROM stdin;
    public          postgres    false    220   x�       i          0    66688    client 
   TABLE DATA           4   COPY public.client (id_client, user_id) FROM stdin;
    public          postgres    false    222   �       l          0    66702    hall 
   TABLE DATA           a   COPY public.hall (id_hall, capacity_hall, id_type_hall, area, is_active, image_path) FROM stdin;
    public          postgres    false    226   j�       m          0    66707 	   hall_type 
   TABLE DATA           Q   COPY public.hall_type (id_type_hall, name_type_hall, type_hall_desc) FROM stdin;
    public          postgres    false    227   C�       p          0    66718    lesson 
   TABLE DATA           �   COPY public.lesson (id_lesson, id_hall, lesson_date, id_trainer, id_type_lesson, id_abonement_client, start_time, end_time) FROM stdin;
    public          postgres    false    231   N�       r          0    66722    lesson_type 
   TABLE DATA           Y   COPY public.lesson_type (id_type_lesson, type_lesson_desc, type_lesson_name) FROM stdin;
    public          postgres    false    233   =�       u          0    66736    role 
   TABLE DATA           (   COPY public.role (id, name) FROM stdin;
    public          postgres    false    237   ��       s          0    66727    trainer 
   TABLE DATA           ]   COPY public.trainer (id_trainer, trainer_education, trainer_experiance, user_id) FROM stdin;
    public          postgres    false    234   �       k          0    66692    users 
   TABLE DATA           �   COPY public.users (user_id, name_user, lastname_user, middle_name, email, phone_number, role_id, age_user, login, password) FROM stdin;
    public          postgres    false    224   J�       y          0    66750    visit 
   TABLE DATA           U   COPY public.visit (id_visit, date_visit, date_time, id_abonement_client) FROM stdin;
    public          postgres    false    243   ��       {          0    66754    visit_services 
   TABLE DATA           [   COPY public.visit_services (id_visit_services, id_visit, id_time, id_services) FROM stdin;
    public          postgres    false    245   %�       �           0    0 (   abonement_client_id_abonement_client_seq    SEQUENCE SET     X   SELECT pg_catalog.setval('public.abonement_client_id_abonement_client_seq', 111, true);
          public          postgres    false    218            �           0    0    abonement_id_abonement_seq    SEQUENCE SET     I   SELECT pg_catalog.setval('public.abonement_id_abonement_seq', 16, true);
          public          postgres    false    219            �           0    0 #   additional_services_id_services_seq    SEQUENCE SET     R   SELECT pg_catalog.setval('public.additional_services_id_services_seq', 13, true);
          public          postgres    false    221            �           0    0    client_id_client_seq    SEQUENCE SET     C   SELECT pg_catalog.setval('public.client_id_client_seq', 21, true);
          public          postgres    false    223            �           0    0    hall_id_hall_seq    SEQUENCE SET     ?   SELECT pg_catalog.setval('public.hall_id_hall_seq', 11, true);
          public          postgres    false    229            �           0    0    hall_type_id_type_hall_seq    SEQUENCE SET     H   SELECT pg_catalog.setval('public.hall_type_id_type_hall_seq', 7, true);
          public          postgres    false    230            �           0    0    lesson_id_lesson_seq    SEQUENCE SET     C   SELECT pg_catalog.setval('public.lesson_id_lesson_seq', 72, true);
          public          postgres    false    232            �           0    0    lesson_type_id_type_lesson_seq    SEQUENCE SET     M   SELECT pg_catalog.setval('public.lesson_type_id_type_lesson_seq', 13, true);
          public          postgres    false    236            �           0    0    role_id_seq    SEQUENCE SET     9   SELECT pg_catalog.setval('public.role_id_seq', 4, true);
          public          postgres    false    238            �           0    0    trainer_id_trainer_seq    SEQUENCE SET     E   SELECT pg_catalog.setval('public.trainer_id_trainer_seq', 11, true);
          public          postgres    false    240            �           0    0    users_user_id_seq    SEQUENCE SET     @   SELECT pg_catalog.setval('public.users_user_id_seq', 58, true);
          public          postgres    false    242            �           0    0    visit_id_visit_seq    SEQUENCE SET     A   SELECT pg_catalog.setval('public.visit_id_visit_seq', 15, true);
          public          postgres    false    244            �           0    0 $   visit_services_id_visit_services_seq    SEQUENCE SET     S   SELECT pg_catalog.setval('public.visit_services_id_visit_services_seq', 1, false);
          public          postgres    false    246            �           2606    66772 &   abonement_client abonement_client_pkey 
   CONSTRAINT     u   ALTER TABLE ONLY public.abonement_client
    ADD CONSTRAINT abonement_client_pkey PRIMARY KEY (id_abonement_client);
 P   ALTER TABLE ONLY public.abonement_client DROP CONSTRAINT abonement_client_pkey;
       public            postgres    false    217            �           2606    66774    abonement abonement_pkey 
   CONSTRAINT     `   ALTER TABLE ONLY public.abonement
    ADD CONSTRAINT abonement_pkey PRIMARY KEY (id_abonement);
 B   ALTER TABLE ONLY public.abonement DROP CONSTRAINT abonement_pkey;
       public            postgres    false    216            �           2606    66776 ,   additional_services additional_services_pkey 
   CONSTRAINT     s   ALTER TABLE ONLY public.additional_services
    ADD CONSTRAINT additional_services_pkey PRIMARY KEY (id_services);
 V   ALTER TABLE ONLY public.additional_services DROP CONSTRAINT additional_services_pkey;
       public            postgres    false    220            �           2606    66778    client client_pkey 
   CONSTRAINT     W   ALTER TABLE ONLY public.client
    ADD CONSTRAINT client_pkey PRIMARY KEY (id_client);
 <   ALTER TABLE ONLY public.client DROP CONSTRAINT client_pkey;
       public            postgres    false    222            �           2606    66780    hall hall_pkey 
   CONSTRAINT     Q   ALTER TABLE ONLY public.hall
    ADD CONSTRAINT hall_pkey PRIMARY KEY (id_hall);
 8   ALTER TABLE ONLY public.hall DROP CONSTRAINT hall_pkey;
       public            postgres    false    226            �           2606    66782    hall_type hall_type_pkey 
   CONSTRAINT     `   ALTER TABLE ONLY public.hall_type
    ADD CONSTRAINT hall_type_pkey PRIMARY KEY (id_type_hall);
 B   ALTER TABLE ONLY public.hall_type DROP CONSTRAINT hall_type_pkey;
       public            postgres    false    227            �           2606    66784    lesson lesson_pkey 
   CONSTRAINT     W   ALTER TABLE ONLY public.lesson
    ADD CONSTRAINT lesson_pkey PRIMARY KEY (id_lesson);
 <   ALTER TABLE ONLY public.lesson DROP CONSTRAINT lesson_pkey;
       public            postgres    false    231            �           2606    66786    lesson_type lesson_type_pkey 
   CONSTRAINT     f   ALTER TABLE ONLY public.lesson_type
    ADD CONSTRAINT lesson_type_pkey PRIMARY KEY (id_type_lesson);
 F   ALTER TABLE ONLY public.lesson_type DROP CONSTRAINT lesson_type_pkey;
       public            postgres    false    233            �           2606    66788    role role_name_key 
   CONSTRAINT     M   ALTER TABLE ONLY public.role
    ADD CONSTRAINT role_name_key UNIQUE (name);
 <   ALTER TABLE ONLY public.role DROP CONSTRAINT role_name_key;
       public            postgres    false    237            �           2606    66790    role role_pkey 
   CONSTRAINT     L   ALTER TABLE ONLY public.role
    ADD CONSTRAINT role_pkey PRIMARY KEY (id);
 8   ALTER TABLE ONLY public.role DROP CONSTRAINT role_pkey;
       public            postgres    false    237            �           2606    66792    trainer trainer_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.trainer
    ADD CONSTRAINT trainer_pkey PRIMARY KEY (id_trainer);
 >   ALTER TABLE ONLY public.trainer DROP CONSTRAINT trainer_pkey;
       public            postgres    false    234            �           2606    66794    users users_email_key 
   CONSTRAINT     Q   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_email_key UNIQUE (email);
 ?   ALTER TABLE ONLY public.users DROP CONSTRAINT users_email_key;
       public            postgres    false    224            �           2606    66796    users users_pkey 
   CONSTRAINT     S   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);
 :   ALTER TABLE ONLY public.users DROP CONSTRAINT users_pkey;
       public            postgres    false    224            �           2606    66798    visit visit_pkey 
   CONSTRAINT     T   ALTER TABLE ONLY public.visit
    ADD CONSTRAINT visit_pkey PRIMARY KEY (id_visit);
 :   ALTER TABLE ONLY public.visit DROP CONSTRAINT visit_pkey;
       public            postgres    false    243            �           2606    66800 "   visit_services visit_services_pkey 
   CONSTRAINT     o   ALTER TABLE ONLY public.visit_services
    ADD CONSTRAINT visit_services_pkey PRIMARY KEY (id_visit_services);
 L   ALTER TABLE ONLY public.visit_services DROP CONSTRAINT visit_services_pkey;
       public            postgres    false    245            �           2620    66801 5   abonement_client trg_calculate_end_date_before_insert    TRIGGER     �   CREATE TRIGGER trg_calculate_end_date_before_insert BEFORE INSERT ON public.abonement_client FOR EACH ROW EXECUTE FUNCTION public.calculate_abonement_end_date();
 N   DROP TRIGGER trg_calculate_end_date_before_insert ON public.abonement_client;
       public          postgres    false    295    217            �           2620    66802 5   abonement_client trg_calculate_end_date_before_update    TRIGGER     �   CREATE TRIGGER trg_calculate_end_date_before_update BEFORE UPDATE ON public.abonement_client FOR EACH ROW WHEN (((new.date_start <> old.date_start) OR (new.id_abonement <> old.id_abonement))) EXECUTE FUNCTION public.calculate_abonement_end_date();
 N   DROP TRIGGER trg_calculate_end_date_before_update ON public.abonement_client;
       public          postgres    false    217    217    217    295            �           2606    66803 3   abonement_client abonement_client_id_abonement_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.abonement_client
    ADD CONSTRAINT abonement_client_id_abonement_fkey FOREIGN KEY (id_abonement) REFERENCES public.abonement(id_abonement);
 ]   ALTER TABLE ONLY public.abonement_client DROP CONSTRAINT abonement_client_id_abonement_fkey;
       public          postgres    false    216    217    4771            �           2606    66808 0   abonement_client abonement_client_id_client_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.abonement_client
    ADD CONSTRAINT abonement_client_id_client_fkey FOREIGN KEY (id_client) REFERENCES public.client(id_client) ON DELETE CASCADE;
 Z   ALTER TABLE ONLY public.abonement_client DROP CONSTRAINT abonement_client_id_client_fkey;
       public          postgres    false    4777    222    217            �           2606    66813    client client_user_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.client
    ADD CONSTRAINT client_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(user_id) ON DELETE CASCADE;
 D   ALTER TABLE ONLY public.client DROP CONSTRAINT client_user_id_fkey;
       public          postgres    false    224    4781    222            �           2606    66818    hall hall_id_type_hall_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.hall
    ADD CONSTRAINT hall_id_type_hall_fkey FOREIGN KEY (id_type_hall) REFERENCES public.hall_type(id_type_hall);
 E   ALTER TABLE ONLY public.hall DROP CONSTRAINT hall_id_type_hall_fkey;
       public          postgres    false    226    4785    227            �           2606    66823 &   lesson lesson_id_abonement_client_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.lesson
    ADD CONSTRAINT lesson_id_abonement_client_fkey FOREIGN KEY (id_abonement_client) REFERENCES public.abonement_client(id_abonement_client);
 P   ALTER TABLE ONLY public.lesson DROP CONSTRAINT lesson_id_abonement_client_fkey;
       public          postgres    false    231    217    4773            �           2606    66828    lesson lesson_id_hall_fkey    FK CONSTRAINT     }   ALTER TABLE ONLY public.lesson
    ADD CONSTRAINT lesson_id_hall_fkey FOREIGN KEY (id_hall) REFERENCES public.hall(id_hall);
 D   ALTER TABLE ONLY public.lesson DROP CONSTRAINT lesson_id_hall_fkey;
       public          postgres    false    226    231    4783            �           2606    66833    lesson lesson_id_trainer_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.lesson
    ADD CONSTRAINT lesson_id_trainer_fkey FOREIGN KEY (id_trainer) REFERENCES public.trainer(id_trainer);
 G   ALTER TABLE ONLY public.lesson DROP CONSTRAINT lesson_id_trainer_fkey;
       public          postgres    false    4791    234    231            �           2606    66838 !   lesson lesson_id_type_lesson_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.lesson
    ADD CONSTRAINT lesson_id_type_lesson_fkey FOREIGN KEY (id_type_lesson) REFERENCES public.lesson_type(id_type_lesson);
 K   ALTER TABLE ONLY public.lesson DROP CONSTRAINT lesson_id_type_lesson_fkey;
       public          postgres    false    4789    231    233            �           2606    66843    trainer trainer_user_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.trainer
    ADD CONSTRAINT trainer_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(user_id);
 F   ALTER TABLE ONLY public.trainer DROP CONSTRAINT trainer_user_id_fkey;
       public          postgres    false    4781    234    224            �           2606    66848    users users_role_id_fkey    FK CONSTRAINT     v   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_role_id_fkey FOREIGN KEY (role_id) REFERENCES public.role(id);
 B   ALTER TABLE ONLY public.users DROP CONSTRAINT users_role_id_fkey;
       public          postgres    false    4795    224    237            �           2606    66853 $   visit visit_id_abonement_client_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.visit
    ADD CONSTRAINT visit_id_abonement_client_fkey FOREIGN KEY (id_abonement_client) REFERENCES public.abonement_client(id_abonement_client);
 N   ALTER TABLE ONLY public.visit DROP CONSTRAINT visit_id_abonement_client_fkey;
       public          postgres    false    217    4773    243            �           2606    66858 .   visit_services visit_services_id_services_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.visit_services
    ADD CONSTRAINT visit_services_id_services_fkey FOREIGN KEY (id_services) REFERENCES public.additional_services(id_services);
 X   ALTER TABLE ONLY public.visit_services DROP CONSTRAINT visit_services_id_services_fkey;
       public          postgres    false    220    4775    245            �           2606    66863 +   visit_services visit_services_id_visit_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.visit_services
    ADD CONSTRAINT visit_services_id_visit_fkey FOREIGN KEY (id_visit) REFERENCES public.visit(id_visit);
 U   ALTER TABLE ONLY public.visit_services DROP CONSTRAINT visit_services_id_visit_fkey;
       public          postgres    false    4797    243    245            c   ]  x��VKnA]�O�$��53���� V�r��1��[�����O2��P}#ޫ���(1���~�^U�S#�+�k㹐�{/�%Ib��o2�Br��S��du�P�ma����9��t}��V�����cYZ.ҫ�@3��6^�y%�s,Ji� ��<�^�8w�*� ����BfRX�Ğ�3l, �\�I���eF>�k��2�{ף7͘�ęƔ�[�"�B�8Sɟ��FX�I�{/�1K`Bl-\���7�6G>'���"�Ś�RsgԲnj#�V6X�\#&w��Q@nq���$�tm��J�����I���
����*�Cݰ���S�c5B a��gD��a�_��Ĭ�!ުA���Jn�S=E�4r�j��.���(R�;g��2�pK?�D��a��VfH��صZK%`�f&榢��VeN/%EԎ'1E�2�Eea�`�'�o�y��ݵ���F�2�K"�J��ܦ�G��U�rv`(�3*��g`@^��%C���YG��t �X�&�� T��^��Ǫ�JvA]�y���r�s#j�!��uH�^�I6��?n+&��kz�ik`\�З�0g�3���+�&롈����X���cL�Ap��@#�({���Ju�cL�L�	��I��s�:��8\s�W��+ﳁ��J���|,��u¬�"��u4y-M������r�j�)��ݺ��;O�f�_�@��*��SR��v)�B	f�a9�þ�	N�F�p�l��Pni��}W��8#l��8���d��kɶ�{��?Z۟�]��f�x���_�w���G�������/�L���z���7��1.�V��o�E�w���VN�z�V���u0      d   �   x�U�Q� D��.�0(��e��m����/�z#�Cp�Q5���ʖ��:��ԒJF�>JZI�A��,Yw�t����-��KZI#'gHe�l8��(_Ӂ !��+>:!B�w��c:W��{"rY���uL�u�	��0S�R)������k5�Ͼ�҅Y
���P��"^��a�lU]      g   �  x��R�N�P\�~E@Ң$�t��m7�TR�bZ$qW
F�.����J��R��?r�-����@�s�3g���)�CZP,�C�=r@���`J���bȩ�4����@�u�X�n���ܮg����8�ا�];:ii��[fszf��<,��3e�Z^a`J:�+�N3���5�]P�sV���\N�����g�8��*���*�;p��n�#���"��M����`(#�aOV���A�m���]g{�jX����k$Ǣn�Y7@g��A�Wq0��r�ilf�{��R�*LTa^�BϷ]����2҆�'�T�`η��|q����c5G	o��
$�@aίW��]&��2e�ך��e��1b�N|)h^����@mg�4M�RcѮ      i   E   x����0�7���IY>zI�u$�DMi3ui�,��rs�Õ/����Q���T�m� C��Շ�>[��      l   �   x�m�K
�0���0�I'P7�
����h+�/��ޠ��R=��FN���,2a���L8x.p�5�R�U,U�еW�A�α�� � .-�҉�B�\XPT�bS3R�����E֥ҙ΢��:���2���ÈC���N��'i[J�[�.���ӏ
��Gx�>��vT[s3�i�����\���9���c��/z�      m   �  x��Umn�@��O��H��r�HBR��W��hԞ����+��Qgfmcb�TE��o��y��oG��b?��eV����V�����*[[����%X-��K�;ˏa~��[�=[����]9��a BRGlV8�[���0�/��?b� x�g��l�g?��27����[�O���w~�:�L&��op���%|�'�~p�9�=cm�23��He8��~��<$X����eH�Х6k��ߞ����{?W�FD)#��.�ւ���ҍ�ƜD����G��V;;��{� �O�)$<��W��Q�@�ݑERp������W�pH:�E��-M�4:�c�W��ȞT�b��Bt���<���7S,d>&�~�߱��5�s����������������Y.KEy'����q.L�,����Ζ���F�k��)�������漡e�oDp,�/�^䄵�Qi��פ;���aJ�֮l�W[a�"g�k�5�ݝ�'fJ�"y-���Ⱦ�Iҝ�3�2^\|��N����Vy����$��a�׃[U���7$�J��a@�jĳ�\f�������D�:G�j��`[�9���⬦�aQ���^���4Ng8/0�n��(��/����Hު8��_������Ț�������Қ�ּ_�f��3enHY3^_�?T����ɏ���Lkr�l����ˤ��>�7z
���So�DzL��ƿ�,gqЋ�_�L$��gF[)4�+�ڎ�L�(��j_c      p   �  x�}U[�1��܅UMbO��ϱM۔�+�`��ı"$����Bm�h��d�rE��&���7@�@����?��c��PXǗ+Jk�i0"��h�ɉi@V1���h�|��ΘX�t�@Yb�mv2Vߣ�UL5F���G�=ji7�	ē���YZ���V�����>t��2����1�9Fc+-'�m��|�y�x�Z�g̫G4F2��{�k����H��hSy�G�Yz��H3oW/�������YX,�P-��H�����c4�\�+�u��ݙ�gis�wg�'���^МIۺVg!����a?)�=��An��l�`M��ٸ��6�cȾv:'��&���|�J��+��,��vM���V��6�����1�L��� f?H��fg<��!s���[ʐ�=���(!�2(���t.�*���|�ԩ!���Gq1�e�GԿ���?:9�_I��;a�l�,>�)�|W^?�u�W�4�      r   u  x��U�r�@<�~�~ �Px�
���Gv�2��	r ��Ă���/��ݳ�mb;)W�53==�����7�e!K��Q�)�%��L�!$��|��@��#=�
�(U`�\2As��8=�o�̤Dl�%;�jʍ%У�?�	z���/� ��A��q�>�
w2��Hl)��-H�+g�
�0�1F�k�:�`�a�k�:�!���B(�ǫ���V�tc26ב�0�,��.к4in(�l�*�R�ѹ1*9�������� �����l��zF��9~karЎ�kW�xE��o߼~�;��90����Я�%Oܫ?4�AmH
S7ҬtqrFZ$M��K�-��ۮ�c����2��d��)�5E#� <����ԇ�MZ�qO�[�9��p�3�����i_̓��u�w��/&.�K	#�k�Ȇ�J�������t��w�r�+n��9
k��^��}lM�SV�o~��ᗔ!6�n���P�Y{"���a\NM�K$o��m��&*YLz4+׊�-!o_*z��ǿ�}���Pfo����3�,`o�	���Wu䳃��W�քI�+��;�����s�>!��l��(J�vl!7v:����,z3Y/y��&�O�f������� /i"      u   B   x�3估���;.l���b��Ŧ� ΅���9/�p.l��,d�ya���e/W� j&�      s   &  x���MN�P���N�	��3�a<� �%��+��<g$À��od�c1�����;<i��ޡCaƻ��q�k�������Oi�>�+6����V�>E����Zk�;me+�Ko���,���fy�3ZR�]*x	+m#��'�S����?�5�Ҧ�RH�q�����d������nfTO�ę�
�B�g��ȵ��U���,YЙ����խ��	S)�Z��d�l\!x�ia��ψ�iݎY��َ%�2�$s��[~_���za��k���n�,%)��s��e      k   5	  x��X�r��}�~�*��mۀ���+U�B���Oӝ��C�2=���Փ�$]y�K���θ�m����#!#1S�������{���X����ٿ]�\���	�G��?�+�����������`U���	QW�K�������D�L��#S'�<������Cdb�h�C
E�h|<9�V�3�|ְ.��EM٢��_��S���z��杚-Pd����߭����ׄ�w�!��K����n�{���d��M�\�.z������!�UC|O28�\��J?W�8�h��i%�ux�12K��1�����;�,������� �0�-s������.m�-0�&�5C�>�k���E�֜bA]�۲]��
������\ ���)��զ�7�l!�@i������R2<�Y4�w�er�n�fT�:C/gGʨ���!��/���yق��`�6L>reK�,yi�"i��\���2%wޕo��umYp��c�ZWgS���>�$9�\׸���b֮��o�ƻ	��	��ܔ�#K␭h�n��|�%�<,�`��������*�Uٝ�&'�	�=dH����k������*��Ǎjq��l�C�h}+�9�.{\���F /�gy�\}ڣ�W���ܑ���>k��IuP%�i�+6�}���P�� ���x�N�@�B֑�E�\�`���+L�!Ҥe2��A�]��U�s��r\����%UF��_����ˠ�nCU�	f�]��mcF��T�H��P͕� z[_�#6��pW2D5��3���A��G�yU��UY#i���b��o�߆�b�$1�v��?9
|����o�˲�q��p�����p����1Þ�ǇT��=?Z�9���vY �(�MT�m�<�؀ѯ!а�K���c�mf��&	�(��Yߛ��d�N����s���;$�.��}�����I8�̚ʦ�v5��j�R�f�ͦ����]L����R{9�'t�O�z'&I�O*~��T�-�;�,�1����l!q�I&&�<�gl/�i�T�.���[�sTcε�RNHL���� �g<1u�����`A=<�G̥�-��5����d�XCr�v+�ps�(��j�wTof,��1Tŝ^�,I����K,l�s(����7m$��w����2,�j�v��9��k'}��5�f#o�¸V���ǕI����
��@��CԄI� ��-4�H�fc��!6�*�>�f�{�X9�"�����������_h�H:�)V��h�<��8����I���bQ��
oe�Ǚ�6�c��VQ��^�Ԧ��ª@�F�.P�}�I߀~��w��=U1�(��dxiu!Zr���
�%ƒ�w3TM�v��({2����3Y��j�̄�م���i���H/������]@�-|}�JR�����Pt��;A2q����Ed��T�1%C��s)��:5�����i��sx�C��῎{�m0�ﶝ��2J聊�d`�dr�H�m-��=����y�"'W�V]���s�p��.e���>��cP��x���D\u�������L6f-�\#����M�n#�h���hsg�)����{$L	Ck<�М����:�2n/`p����f�a24aZ��y�{R"7�"}d&W/,JE�h��ggz���1�v� {�������T�d-��Np�R-o�P��|�ey��Yv�����Jf�t�Y�#�t�dqԴ*�v�M�~��z }�D��Éi�FPbub�
a�e�0t��3����ɋ�m�Gc��tF�v�$�������ܿ���8w�N����(y�+n(�&u�d(����(]��9M��Ur���l�xgBBB ��$0���a.���v%GCA��4&�3�}r"�Ɗ�3�e����v6�کZ��PP��F��e.KW$���Xpc���>�ÖXWC��?�K�������DÆ/#)8�6>��i�E�sV���bY�5N�N�5�̲':�<5<ȟk%A��4��!ء4�N�P�5�q�Cpֲ5��y��.歋樟�Ţ�8�_�T�UI&4�`"B��'hʟ�����@�4��j	(�N���M�1�(�&����z�Y���P�Z'GeSL�ZdN�ژ��8����?�;
�`��3��px�P�|lQ��:��sb�ꀫ�yQ�ZW+);kN��9�"�L��1�yr!Ə��T.�@l�&�ڑ�j�)UΠ�e��Ɨ�^K>�'�C2���"����}�aHW>?�x*���a�7B����f��,��Vq������땧��U�)��7v�����bR��WO�ӯ���ߥI��?�i��      y   �   x�E��� �3�ew
J/�?^>��B����V��D��U�klYfZ/�m��6rL��n���N�
g�X�f��5���E��wL������r���)�i�X�4�飘����i�x����tR�~��}���4A      {      x������ � �     