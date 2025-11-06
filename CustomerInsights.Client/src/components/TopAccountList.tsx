import {Avatar, Button, Flex, Table} from "@radix-ui/themes";
import {EllipsisIcon} from "lucide-react";

interface TopAccount {
    id: string;
    name: string;
    score: number;
}

const ContactList: React.FC = () => {

    const accounts= [
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
    ] as TopAccount[];

    return (

        <Flex direction="column" gap="3">
            <Table.Root>
                <Table.Header>
                    <Table.Row>
                        <Table.ColumnHeaderCell>Name</Table.ColumnHeaderCell>
                        <Table.ColumnHeaderCell>Score</Table.ColumnHeaderCell>
                    </Table.Row>
                </Table.Header>
                <Table.Body>
                    {accounts.map((a, i) => (
                        <Table.Row key={i}>
                            <Table.Cell>{a.name}</Table.Cell>
                            <Table.Cell>{a.score}</Table.Cell>
                        </Table.Row>
                    ))}
                </Table.Body>
            </Table.Root>
        </Flex>
    );
};

export default ContactList;