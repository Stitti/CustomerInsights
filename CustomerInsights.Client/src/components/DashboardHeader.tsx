import {Card, Flex, IconButton, Select} from "@radix-ui/themes";
import {RefreshCcw} from "lucide-react";

export function DashboardHeader() {
    return (
        <Card style={{ flex: 1, minWidth: "100%" }} variant="surface"  mb="4">
            <Flex style={{ alignItems: "center", justifyContent: "space-between", paddingLeft: "0.25rem", paddingRight: "0.25rem" }}>
                <Select.Root defaultValue="total">
                    <Select.Trigger variant="soft" style={{width:'10%'}} />
                    <Select.Content variant="soft">
                        <Select.Item value="total">Total</Select.Item>
                        <Select.Item value="this_week">This week</Select.Item>
                        <Select.Item value="this_month">This month</Select.Item>
                        <Select.Item value="this_year">This year</Select.Item>
                        <Select.Separator />
                        <Select.Item value="last_week">Last week</Select.Item>
                        <Select.Item value="last_month">Last month</Select.Item>
                        <Select.Item value="last_year">Last year</Select.Item>
                    </Select.Content>
                </Select.Root>

                <IconButton variant="soft">
                    <RefreshCcw size="20"/>
                </IconButton>
            </Flex>
        </Card>
    )
}