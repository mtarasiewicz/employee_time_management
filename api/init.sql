CREATE EXTENSION IF NOT EXISTS pgcrypto;


CREATE TABLE IF NOT EXISTS public.employees
(
    id uuid NOT NULL DEFAULT gen_random_uuid(), 
    firstname character varying(50) NOT NULL,    
    lastname character varying(50) NOT NULL,   
    email character varying(100) NOT NULL,  
    CONSTRAINT employees_pkey PRIMARY KEY (id), 
    CONSTRAINT employees_email_key UNIQUE (email) 
);

CREATE TABLE IF NOT EXISTS public.timeentries
(
    id uuid NOT NULL DEFAULT gen_random_uuid(), 
    employeeid uuid NOT NULL,                   
    date date NOT NULL,                          
    hoursworked integer NOT NULL CHECK (hoursworked >= 1 AND hoursworked <= 24),  
    CONSTRAINT timeentries_pkey PRIMARY KEY (id),
    CONSTRAINT timeentries_employeeid_fkey FOREIGN KEY (employeeid)
        REFERENCES public.employees (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
);



CREATE UNIQUE INDEX IF NOT EXISTS unique_employee_date
ON public.timeentries(employeeid, date);