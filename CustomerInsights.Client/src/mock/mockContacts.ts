import type {Contact} from "../models/contact.ts";
import { v4 as uuidv4 } from "uuid";

export const mockContacts: Contact[] = [
    {
        id: uuidv4(),
        firstname: "Anna",
        lastname: "Müller",
        email: "anna.mueller@example.com",
        phone: "+49 170 1234567",
        company: "Müller Consulting GmbH",
    },
    {
        id: uuidv4(),
        firstname: "Jonas",
        lastname: "Schneider",
        email: "jonas.schneider@techflow.io",
        phone: "+49 160 9988776",
        company: "TechFlow Solutions AG",
    },
    {
        id: uuidv4(),
        firstname: "Sophie",
        lastname: "Berger",
        email: "sophie.berger@greenfoods.de",
        phone: "+49 152 2233445",
        company: "GreenFoods GmbH",
    },
    {
        id: uuidv4(),
        firstname: "Michael",
        lastname: "Weber",
        email: "michael.weber@medisoft.com",
        phone: "+49 176 3344556",
        company: "MediSoft AG",
    },
    {
        id: uuidv4(),
        firstname: "Laura",
        lastname: "Fischer",
        email: "laura.fischer@designhaus.de",
        phone: "+49 151 4455667",
        company: "DesignHaus Studios",
    },
    {
        id: uuidv4(),
        firstname: "Peter",
        lastname: "Klein",
        email: "peter.klein@buildsmart.de",
        phone: "+49 175 7788990",
        company: "BuildSmart GmbH",
    },
    {
        id: uuidv4(),
        firstname: "Julia",
        lastname: "Hoffmann",
        email: "julia.hoffmann@finovate.io",
        phone: "+49 171 9988775",
        company: "Finovate Solutions",
    },
    {
        id: uuidv4(),
        firstname: "David",
        lastname: "Krüger",
        email: "david.krueger@autohaus-meier.de",
        phone: "+49 172 8899776",
        company: "Autohaus Meier KG",
    },
];