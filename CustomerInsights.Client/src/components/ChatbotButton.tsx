import {BotIcon} from "lucide-react";

interface Props {
    onClick: () => void;
}

export default function ChatbotButton(props: Props) {


    return (
        <button className="hover-button" onClick={props.onClick}>
            <BotIcon />
        </button>
    )
}