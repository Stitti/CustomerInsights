import {Badge, Card, Flex, Text} from "@radix-ui/themes";

interface Props {
    title: string;
    value: string;
    trend: number;
}

export function MetricCard({ title, value, trend }: Props) {

    return (
        <Card style={{ flex: 1, padding: "1rem" }} variant="surface" mb="4">
            <Text size="2" weight="bold">
                {title}
            </Text>

            <Flex style={{ justifyContent: "space-between", marginTop: "1rem" }}>
                <Text size="5">{value}</Text>
                <Badge color="green">
                    <Text>{trend}%</Text>
                </Badge>
            </Flex>
        </Card>
    )
}