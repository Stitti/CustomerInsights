import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Flex, Table } from "@radix-ui/themes";
const ContactList = () => {
    const accounts = [
        {
            id: "1",
            name: "ASC",
            score: 90
        },
        {
            id: "2",
            name: "Microsoft",
            score: 45
        },
        {
            id: "3",
            name: "Orcale",
            score: 56
        }
    ];
    return (_jsx(Flex, { direction: "column", gap: "3", children: _jsxs(Table.Root, { children: [_jsx(Table.Header, { children: _jsxs(Table.Row, { children: [_jsx(Table.ColumnHeaderCell, { children: "Name" }), _jsx(Table.ColumnHeaderCell, { children: "Score" })] }) }), _jsx(Table.Body, { children: accounts.map((a, i) => (_jsxs(Table.Row, { children: [_jsx(Table.Cell, { children: a.name }), _jsx(Table.Cell, { children: a.score })] }, i))) })] }) }));
};
export default ContactList;
