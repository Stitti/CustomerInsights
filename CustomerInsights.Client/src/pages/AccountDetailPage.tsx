import {
    Avatar,
    Box,
    Card,
    Code,
    DataList,
    Flex, Heading,
    IconButton,
    Select,
    Table,
    Text,
    TextField,
} from "@radix-ui/themes";
import { CopyIcon } from "lucide-react";
import {DashboardHeader} from "../components/DashboardHeader";
import {MetricsTrends} from "../components/MetricsTrends";
import ModifiedCard from "../components/ModifiedCard";

export function AccountDetailPage() {
    return (
        <Box flexGrow="1" p="6">
        <Flex gap="3" direction="column" wrap="wrap">
            <DashboardHeader/>
            <MetricsTrends/>
            <Card style={{ flex: 1, minWidth: "100%" }} variant="ghost" mb="4">
                <Flex direction="row" gap="8" wrap="wrap">
                    <Card style={{ flex: 1, minWidth: 320 }} variant="surface" size="3" mb="6">
                        <DataList.Root size="3">
                            <DataList.Item align="center">
                                <DataList.Label>Name</DataList.Label>
                                <DataList.Value style={{ flex: 1 }}>
                                    <TextField.Root
                                        style={{ width: "100%" }}
                                        disabled
                                        type="text"
                                        value="TechFlow Solutions AG"
                                    />
                                </DataList.Value>
                            </DataList.Item>

                            <DataList.Item>
                                <DataList.Label minWidth="88px">Classification</DataList.Label>
                                <DataList.Value style={{ flex: 1 }}>
                                    <Select.Root defaultValue="a" disabled value="a">
                                        <Select.Trigger style={{ width: "100%" }} />
                                        <Select.Content>
                                            <Select.Item value="a">A</Select.Item>
                                            <Select.Item value="b">B</Select.Item>
                                            <Select.Item value="c">C</Select.Item>
                                        </Select.Content>
                                    </Select.Root>
                                </DataList.Value>
                            </DataList.Item>

                            <DataList.Item>
                                <DataList.Label minWidth="88px">Industry</DataList.Label>
                                <DataList.Value style={{ flex: 1 }}>
                                    <Select.Root defaultValue="software_it" disabled value="software_it">
                                        <Select.Trigger style={{ width: "100%" }} />
                                        <Select.Content>
                                            <Select.Item value="software_it">Software & IT</Select.Item>
                                            <Select.Item value="construction">Construction</Select.Item>
                                            <Select.Item value="healthcare">Healthcare</Select.Item>
                                        </Select.Content>
                                    </Select.Root>
                                </DataList.Value>
                            </DataList.Item>

                            <DataList.Item>
                                <DataList.Label minWidth="88px">Country</DataList.Label>
                                <DataList.Value style={{ flex: 1 }}>
                                    <TextField.Root
                                        style={{ width: "100%" }}
                                        disabled
                                        type="text"
                                        value="Germany"
                                    />
                                </DataList.Value>
                            </DataList.Item>

                            <DataList.Item>
                                <DataList.Label minWidth="88px">External ID</DataList.Label>
                                <DataList.Value style={{ flex: 1 }}>
                                    <Flex align="center" gap="2">
                                        <Code variant="ghost">u_2J89JSA4GJ</Code>
                                        <IconButton
                                            size="1"
                                            aria-label="Copy value"
                                            color="gray"
                                            variant="ghost"
                                        >
                                            <CopyIcon size="16" />
                                        </IconButton>
                                    </Flex>
                                </DataList.Value>
                            </DataList.Item>
                        </DataList.Root>
                    </Card>
                </Flex>
            </Card>

            <Card variant="surface" size="3" mb="6">
                <Heading size="5" mb="3">Contacts</Heading>
                <Table.Root>
                    <Table.Header>
                        <Table.Row>
                            <Table.ColumnHeaderCell>Name</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Email</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Phone</Table.ColumnHeaderCell>
                        </Table.Row>
                    </Table.Header>
                    <Table.Body>
                        <Table.Row>
                            <Table.Cell>Jonas Schneider</Table.Cell>
                            <Table.Cell>jonas.schneider@techflow.io</Table.Cell>
                            <Table.Cell>+49 160 9988776</Table.Cell>
                        </Table.Row>
                    </Table.Body>
                </Table.Root>
            </Card>

            <Card variant="surface" size="3" mb="6">
                <Heading size="5" mb="3">Interactions</Heading>
                <Table.Root>
                    <Table.Header>
                        <Table.Row>
                            <Table.ColumnHeaderCell>Title</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Channel</Table.ColumnHeaderCell>
                            <Table.ColumnHeaderCell>Occured At</Table.ColumnHeaderCell>
                        </Table.Row>
                    </Table.Header>
                    <Table.Body>
                        <Table.Row>
                            <Table.Cell>Follow-up zum Angebot Q4</Table.Cell>
                            <Table.Cell>E-Mail</Table.Cell>
                            <Table.Cell>2025-10-28T14:00:00Z</Table.Cell>
                        </Table.Row>
                    </Table.Body>
                </Table.Root>
            </Card>
            <ModifiedCard/>
        </Flex>
        </Box>
    );
}