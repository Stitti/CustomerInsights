import type {Account} from "../models/account";
import { v4 as uuidv4 } from "uuid";

export const mockAccounts: Account[] = [
    {
        id: uuidv4(),
        name: "TechFlow Solutions AG",
        classification: "A",
        industry: "Software & IT",
        country: "Germany",
    },
    {
        id: uuidv4(),
        name: "GreenFoods GmbH",
        classification: "B",
        industry: "Food & Beverage",
        country: "Germany",
    },
    {
        id: uuidv4(),
        name: "BuildSmart GmbH",
        classification: "C",
        industry: "Construction",
        country: "Austria",
    },
    {
        id: uuidv4(),
        name: "MediSoft AG",
        classification: "A",
        industry: "Healthcare",
        country: "Switzerland",
    },
    {
        id: uuidv4(),
        name: "Finovate Solutions",
        classification: "B",
        industry: "Financial Services",
        country: "Germany",
    },
    {
        id: uuidv4(),
        name: "Autohaus Meier KG",
        classification: "B",
        industry: "Automotive",
        country: "Germany",
    },
    {
        id: uuidv4(),
        name: "DesignHaus Studios",
        classification: "C",
        industry: "Creative Services",
        country: "Austria",
    },
    {
        id: uuidv4(),
        name: "Nordic Logistics AG",
        classification: "B",
        industry: "Transportation & Logistics",
        country: "Denmark",
    },
    {
        id: uuidv4(),
        name: "Helio Energy Systems",
        classification: "A",
        industry: "Renewable Energy",
        country: "Germany",
    },
    {
        id: uuidv4(),
        name: "EduSmart Learning GmbH",
        classification: "C",
        industry: "Education Technology",
        country: "Switzerland",
    },
];